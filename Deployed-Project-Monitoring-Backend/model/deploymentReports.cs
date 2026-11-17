
namespace Models;

public class DeploymentReports
{
    public int id { get; set; }

    public int project_id { get; set; }

    public string version { get; set; } = string.Empty;

    public string environment { get; set; } = "Production";

    public string? deployment_id { get; set; }

    public string? commit_hash { get; set; }

    public DateTime deployed_at { get; set; } = DateTime.UtcNow;

    public DateTime received_at { get; set; } = DateTime.UtcNow;

    public Projects? Project { get; set; }
}
