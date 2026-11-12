using Enums;
namespace Models;
public class Projects
{
    public int id { get; set; }
    public int user_id { get; set; }
    public string project_name { get; set; } = string.Empty;
    public string provider { get; set; } = string.Empty;
    public string deployment_url { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
    public DateTime last_checked_at { get; set; }
    public DeploymentStatus status { get; set; } = DeploymentStatus.Unknown;

    public Users? User { get; set; }

}
