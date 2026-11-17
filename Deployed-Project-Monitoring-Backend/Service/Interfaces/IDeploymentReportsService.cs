
using DTOs;
using Models;

namespace SDKInterface;

public interface IDeploymentReportService
{
    Task<DeploymentReports?> ReportDeployment(
        DeploymentReportRequest request
    );
    Task<List<DeploymentReports>> GetDeploymentHistory(int projectId);
}
