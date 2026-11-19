using System.ComponentModel.DataAnnotations;

namespace DTOs;

public class HeartbeatRequest
{
    [MaxLength(64)]
    public string? instance_id { get; set; }

    [MaxLength(100)]
    public string? version { get; set; }

    [MaxLength(50)]
    public string? environment { get; set; }

    [MaxLength(30)]
    public string? node_version { get; set; }

    public long? uptime_seconds { get; set; }
    public double? memory_mb { get; set; }
    public double? cpu_percent { get; set; }
    public int? interval_seconds { get; set; }
}

public class HeartbeatInstanceInfo
{
    public string instance_id { get; set; } = string.Empty;
    public string? version { get; set; }
    public bool is_alive { get; set; }
    public DateTime started_at { get; set; }
    public DateTime last_heartbeat_at { get; set; }
    public long uptime_seconds { get; set; }
    public double? memory_mb { get; set; }
    public double? cpu_percent { get; set; }
}

public class HeartbeatStatusResponse
{
    public int project_id { get; set; }

    /// <summary>NoData | Alive | Missed</summary>
    public string state { get; set; } = "NoData";

    /// <summary>
    /// Combines the external ping with the SDK heartbeat:
    /// Healthy | AppRunningButUnreachable | AppLikelyDown | SdkNotReporting | NoSdkData | Unknown
    /// </summary>
    public string diagnosis { get; set; } = "Unknown";

    public DateTime? last_heartbeat_at { get; set; }
    public int alive_instances { get; set; }

    public string? latest_deployed_version { get; set; }
    public List<string> running_versions { get; set; } = new();

    /// <summary>True when a running instance reports a different version than the latest deployment report.</summary>
    public bool version_drift { get; set; }

    /// <summary>Instances that started in the last 15 minutes.</summary>
    public int recent_starts { get; set; }

    /// <summary>2+ instances started in the last 15 minutes and are already gone.</summary>
    public bool possible_crash_loop { get; set; }

    public List<HeartbeatInstanceInfo> instances { get; set; } = new();
}