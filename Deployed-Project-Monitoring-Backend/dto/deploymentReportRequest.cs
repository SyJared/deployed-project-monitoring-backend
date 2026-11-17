
using System.ComponentModel.DataAnnotations;

namespace DTOs;

public class DeploymentReportRequest
{
    [Required]
    [MaxLength(50)]
    public string version { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string environment { get; set; } = "Production";

    [MaxLength(100)]
    public string? deployment_id { get; set; }

    [MaxLength(100)]
    public string? commit_hash { get; set; }

    public DateTime? deployed_at { get; set; }
}
