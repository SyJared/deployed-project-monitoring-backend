using Models;

namespace DTOs;
public class ProjectsInfo
{
    public int user_id { get; set; }
    public string project_name { get; set; } = string.Empty;
    public string provider { get; set; } = string.Empty;
    public string deployment_url { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
}
public class ProjectCreationResult
{
    public Projects Project { get; set; } = null!;

    public string ApiKey { get; set; } = string.Empty;
}