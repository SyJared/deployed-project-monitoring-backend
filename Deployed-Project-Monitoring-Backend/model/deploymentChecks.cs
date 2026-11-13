
namespace Models;

public class DeploymentChecks
{
    public int id { get; set; }

    public int project_id { get; set; }

    public DateTime checked_at { get; set; } = DateTime.UtcNow;

    public int? http_status_code { get; set; }

    public long response_time_ms { get; set; }

    public string status { get; set; } = "Unknown";

    public string? error_message { get; set; }

    public Projects? Project { get; set; }
}
