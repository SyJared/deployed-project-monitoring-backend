
namespace DTOs;

public class DeploymentCheckResponse
{
    public int id { get; set; }
    public int project_id { get; set; }
    public DateTime checked_at { get; set; }
    public int? http_status_code { get; set; }
    public long response_time_ms { get; set; }
    public string status { get; set; } = string.Empty;
    public string? error_message { get; set; }
}
