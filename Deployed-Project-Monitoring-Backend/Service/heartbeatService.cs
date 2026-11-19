using Data;
using DTOs;
using Microsoft.EntityFrameworkCore;
using Models;
using SDKInterface;
using Security;

namespace SDKServices;

public class HeartbeatService : IHeartbeatService
{
    private const int MissedAfterIntervals = 3;      // alive = heard within 3 intervals
    private const int MinIntervalSeconds = 10;
    private const int MaxIntervalSeconds = 3600;
    private const long MaxUptimeSeconds = 10L * 365 * 24 * 3600;
    private static readonly TimeSpan InstanceRetention = TimeSpan.FromDays(7);
    private static readonly TimeSpan RecentStartWindow = TimeSpan.FromMinutes(15);

    private readonly ApplicationDbContext _context;

    public HeartbeatService(ApplicationDbContext context)
    {
        _context = context;
    }

    // ---------------------------------------------------------------
    // SDK -> server
    // ---------------------------------------------------------------
    public async Task<bool> ReceiveHeartbeat(string apiKey, HeartbeatRequest request)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            return false;

        var apiKeyHash = ApiKeyGenerator.Hash(apiKey);

        var projectId = await _context.Projects
            .Where(p => p.api_key_hash == apiKeyHash)
            .Select(p => (int?)p.id)
            .FirstOrDefaultAsync();

        if (projectId == null)
            return false;

        var pid = projectId.Value;
        var now = DateTime.UtcNow;

        var instanceId = string.IsNullOrWhiteSpace(request.instance_id)
            ? "default"
            : request.instance_id.Trim();

        var uptime = Math.Clamp(request.uptime_seconds ?? 0, 0, MaxUptimeSeconds);
        var interval = Math.Clamp(request.interval_seconds ?? 60, MinIntervalSeconds, MaxIntervalSeconds);

        var heartbeat = await _context.Heartbeats
            .FirstOrDefaultAsync(h => h.project_id == pid && h.instance_id == instanceId);

        var isNew = heartbeat == null;

        if (heartbeat == null)
        {
            heartbeat = new Heartbeats
            {
                project_id = pid,
                instance_id = instanceId,
                first_seen_at = now,
                started_at = now.AddSeconds(-uptime)
            };

            _context.Heartbeats.Add(heartbeat);
        }

        heartbeat.version = request.version?.Trim();
        heartbeat.environment = request.environment?.Trim();
        heartbeat.node_version = request.node_version?.Trim();
        heartbeat.uptime_seconds = uptime;
        heartbeat.memory_mb = CleanMetric(request.memory_mb);
        heartbeat.cpu_percent = CleanMetric(request.cpu_percent);
        heartbeat.interval_seconds = interval;
        heartbeat.last_heartbeat_at = now;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException) when (isNew)
        {
            // Two first heartbeats from the same instance raced on the unique index.
            // The row exists now, so the next heartbeat will simply update it.
            _context.Entry(heartbeat).State = EntityState.Detached;
            return true;
        }

        if (isNew)
        {
            // A new instance appeared (first run or restart): prune this project's old rows.
            var cutoff = now - InstanceRetention;

            await _context.Heartbeats
                .Where(h => h.project_id == pid && h.last_heartbeat_at < cutoff)
                .ExecuteDeleteAsync(); // EF Core 7+
        }

        return true;
    }

    // ---------------------------------------------------------------
    // Dashboard
    // ---------------------------------------------------------------
    public async Task<HeartbeatStatusResponse> GetHeartbeatStatus(int projectId)
    {
        var now = DateTime.UtcNow;

        var instances = await _context.Heartbeats
            .AsNoTracking()
            .Where(h => h.project_id == projectId)
            .OrderByDescending(h => h.last_heartbeat_at)
            .ToListAsync();

        var alive = instances.Where(h => IsAlive(h, now)).ToList();

        var state = instances.Count == 0 ? "NoData"
                  : alive.Count > 0 ? "Alive"
                  : "Missed";

        // Compare versions within the environment being reported by the SDK.
        var environment = (alive.FirstOrDefault() ?? instances.FirstOrDefault())?.environment;

        var latestDeployment = await _context.DeploymentReports
            .AsNoTracking()
            .Where(d => d.project_id == projectId &&
                        (environment == null || d.environment == environment))
            .OrderByDescending(d => d.deployed_at)
            .FirstOrDefaultAsync();

        var runningVersions = alive
            .Select(h => h.version)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .Distinct()
            .ToList();

        var versionDrift = latestDeployment != null &&
            runningVersions.Any(v =>
                NormalizeVersion(v) != NormalizeVersion(latestDeployment.version));

        var recentStarts = instances
            .Where(h => h.started_at >= now - RecentStartWindow)
            .ToList();

        var possibleCrashLoop = recentStarts.Count(h => !IsAlive(h, now)) >= 2;

        // Combine with the external ping result.
        var lastCheckStatus = await _context.DeploymentChecks
            .AsNoTracking()
            .Where(c => c.project_id == projectId)
            .OrderByDescending(c => c.checked_at)
            .Select(c => c.status)
            .FirstOrDefaultAsync();

        var diagnosis = (lastCheckStatus, state) switch
        {
            (_, "NoData") => "NoSdkData",
            ("Online", "Alive") => "Healthy",
            ("Offline", "Alive") => "AppRunningButUnreachable",
            ("Offline", "Missed") => "AppLikelyDown",
            ("Online", "Missed") => "SdkNotReporting",
            _ => "Unknown"
        };

        return new HeartbeatStatusResponse
        {
            project_id = projectId,
            state = state,
            diagnosis = diagnosis,
            last_heartbeat_at = instances.FirstOrDefault()?.last_heartbeat_at,
            alive_instances = alive.Count,
            latest_deployed_version = latestDeployment?.version,
            running_versions = runningVersions,
            version_drift = versionDrift,
            recent_starts = recentStarts.Count,
            possible_crash_loop = possibleCrashLoop,
            instances = instances
                .Take(20)
                .Select(h => new HeartbeatInstanceInfo
                {
                    instance_id = h.instance_id,
                    version = h.version,
                    is_alive = IsAlive(h, now),
                    started_at = h.started_at,
                    last_heartbeat_at = h.last_heartbeat_at,
                    uptime_seconds = h.uptime_seconds,
                    memory_mb = h.memory_mb,
                    cpu_percent = h.cpu_percent
                })
                .ToList()
        };
    }

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------
    private static bool IsAlive(Heartbeats h, DateTime now) =>
        now - h.last_heartbeat_at <= TimeSpan.FromSeconds(h.interval_seconds * MissedAfterIntervals);

    // "v1.2.3", "V1.2.3" and "1.2.3" are the same version.
    private static string NormalizeVersion(string? v) =>
        (v ?? string.Empty).Trim().TrimStart('v', 'V').ToLowerInvariant();

    private static double? CleanMetric(double? value) =>
        value is null || double.IsNaN(value.Value) || double.IsInfinity(value.Value) || value < 0
            ? null
            : Math.Round(value.Value, 1);
}