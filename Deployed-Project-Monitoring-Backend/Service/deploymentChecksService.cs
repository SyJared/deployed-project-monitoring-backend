using Data;
using DeploymentChecksInterface;
using DTOs;
using Enums;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Diagnostics;


namespace Services;

public class DeploymentChecksService : IDeploymentChecksService
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;

    public DeploymentChecksService(
        ApplicationDbContext context,
        HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    public async Task<DeploymentChecks?> CheckProject(int projectId)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.id == projectId);

        if (project == null)
            return null;

        var check = new DeploymentChecks
        {
            project_id = project.id,
            checked_at = DateTime.UtcNow
        };

        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var response = await _httpClient.GetAsync(
                project.deployment_url,
                HttpCompletionOption.ResponseHeadersRead
            );

            stopwatch.Stop();

            check.http_status_code = (int)response.StatusCode;
            check.response_time_ms = stopwatch.ElapsedMilliseconds;

            check.status = response.IsSuccessStatusCode
                ? "Online"
                : "Offline";

            check.error_message = null;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            check.response_time_ms = stopwatch.ElapsedMilliseconds;
            check.status = "Offline";
            check.error_message = ex.Message;
        }

        // Update the project's latest monitoring information
        project.last_checked_at = check.checked_at;

        project.status = check.status == "Online"
            ? DeploymentStatus.Online
            : DeploymentStatus.Offline;

        // Get the previous health check BEFORE saving the current one.
        var previousCheck = await _context.DeploymentChecks
            .Where(c => c.project_id == projectId)
            .OrderByDescending(c => c.checked_at)
            .FirstOrDefaultAsync();

        // Find an existing ongoing downtime event.
        var ongoingDowntime = await _context.DowntimeEvents
            .FirstOrDefaultAsync(d =>
                d.project_id == projectId &&
                d.status == DowntimeStatus.Ongoing);

        // If the current check failed...
        if (check.status == "Offline")
        {
            // Only create an event if the previous check also failed.
            if (ongoingDowntime == null &&
                previousCheck?.status == "Offline")
            {
                var downtime = new DowntimeEvents
                {
                    project_id = projectId,

                    // Use the first failed check as the downtime start.
                    started_at = previousCheck.checked_at,

                    status = DowntimeStatus.Ongoing
                };

                _context.DowntimeEvents.Add(downtime);
            }
        }
        // If the project is online again...
        else if (check.status == "Online")
        {
            if (ongoingDowntime != null)
            {
                ongoingDowntime.resolved_at = check.checked_at;
                ongoingDowntime.status = DowntimeStatus.Resolved;
            }
        }
        // Save the monitoring history
        _context.DeploymentChecks.Add(check);

        await _context.SaveChangesAsync();

        return check;
    }
    public async Task<List<DeploymentChecks>?> CheckAllProjects(int userId)
    {
        var projects = await _context.Projects
            .Where(p => p.user_id == userId)
            .ToListAsync();

        var results = new List<DeploymentChecks>();

        foreach (var project in projects)
        {
            var result = await CheckProject(project.id);

            if (result != null)
            {
                results.Add(result);
            }
        }

        return results;
    }

    public async Task<List<DeploymentChecks>> CheckAllProjects()
    {
        var projects = await _context.Projects
            .Select(p => p.id)
            .ToListAsync();

        var results = new List<DeploymentChecks>();

        foreach (var projectId in projects)
        {
            var result = await CheckProject(projectId);

            if (result != null)
                results.Add(result);
        }

        return results;
    }

    public async Task<UptimeResponse> GetUptime(int projectId, int days)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);

        var checks = await _context.DeploymentChecks
            .Where(c =>
                c.project_id == projectId &&
                c.checked_at >= startDate)
            .ToListAsync();

        var totalChecks = checks.Count;

        var successfulChecks = checks.Count(c => c.status == "Online");

        // Count distinct calendar dates that have health checks
        var daysChecked = checks
            .Select(c => c.checked_at.Date)
            .Distinct()
            .Count();

        var uptime = totalChecks == 0
            ? 0
            : (double)successfulChecks / totalChecks * 100;

        return new UptimeResponse
        {
            project_id = projectId,
            requested_days = days,
            days_checked = daysChecked,
            total_checks = totalChecks,
            successful_checks = successfulChecks,
            uptime_percentage = Math.Round(uptime, 2)
        };
    }
    public async Task<List<DowntimeEvents>> GetProjectDowntimeHistory(
    int projectId)
    {
        return await _context.DowntimeEvents
            .Where(d => d.project_id == projectId)
            .OrderByDescending(d => d.started_at)
            .ToListAsync();
    }
    public async Task<DowntimeEvents?> GetOngoingDowntime(int projectId)
    {
        return await _context.DowntimeEvents
            .FirstOrDefaultAsync(d =>
                d.project_id == projectId &&
                d.status == DowntimeStatus.Ongoing);
    }
}