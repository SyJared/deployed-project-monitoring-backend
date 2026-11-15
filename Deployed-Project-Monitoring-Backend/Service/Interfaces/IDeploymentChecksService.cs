using DTOs;
using Models;
namespace DeploymentChecksInterface;
public interface IDeploymentChecksService
{
    Task<DeploymentChecks?> CheckProject(int projectId);
    Task<List<DeploymentChecks>?> CheckAllProjects(int userId);
    Task<List<DeploymentChecks>?> CheckAllProjects();
    Task<UptimeResponse> GetUptime(int projectId, int days);
}