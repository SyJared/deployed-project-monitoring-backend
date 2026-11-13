using Models;
namespace DeploymentChecksInterface;
public interface IDeploymentChecksService
{
    Task<DeploymentChecks?> CheckProject(int projectId);
}