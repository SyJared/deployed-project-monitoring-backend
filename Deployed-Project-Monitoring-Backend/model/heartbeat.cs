namespace Models;

/// <summary>
/// Latest heartbeat state for ONE running instance of a project's app.
/// One row per (project_id, instance_id). A restarted app gets a new instance_id,
/// and an app scaled to N instances has N rows. Rows older than 7 days are pruned.
/// </summary>
public class Heartbeats
{
    public int id { get; set; }
    public int project_id { get; set; }

    public string instance_id { get; set; } = string.Empty;

    public string? version { get; set; }
    public string? environment { get; set; }
    public string? node_version { get; set; }

    public long uptime_seconds { get; set; }
    public double? memory_mb { get; set; }
    public double? cpu_percent { get; set; }

    /// <summary>How often the SDK promises to send heartbeats. Used to decide when one is "missed".</summary>
    public int interval_seconds { get; set; } = 60;

    /// <summary>Computed on first sight: received_at - uptime.</summary>
    public DateTime started_at { get; set; }
    public DateTime first_seen_at { get; set; }
    public DateTime last_heartbeat_at { get; set; }
}