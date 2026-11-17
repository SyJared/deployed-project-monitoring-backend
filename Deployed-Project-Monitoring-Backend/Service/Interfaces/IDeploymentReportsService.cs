
using DTOs;
using Models;

namespace SDKInterface;

public interface IDeploymentReportService
{
    Task<DeploymentReports?> ReportDeployment(
    string apiKey,
    DeploymentReportRequest request);
    Task<List<DeploymentReports>> GetDeploymentHistory(int projectId);
}
