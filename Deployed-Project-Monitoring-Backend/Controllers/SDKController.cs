
using DTOs;
using Microsoft.AspNetCore.Mvc;
using SDKInterface;

namespace Controllers;

[ApiController]
[Route("api/sdk")]
public class SDKController : ControllerBase
{
    private readonly IDeploymentReportService _deploymentService;

    public SDKController(
        IDeploymentReportService deploymentService)
    {
        _deploymentService = deploymentService;
    }

    [HttpPost("deployments")]
    public async Task<IActionResult> ReportDeployment(
        [FromBody] DeploymentReportRequest request)
    {
        var result = await _deploymentService.ReportDeployment(request);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Project not found."
            });
        }

        return Ok(new
        {
            message = "Deployment reported successfully.",
            deployment = new
            {
                result.id,
                result.project_id,
                result.version,
                result.environment,
                result.deployment_id,
                result.commit_hash,
                result.deployed_at,
                result.received_at
            }
        });
    }
    [HttpGet("deployments/{projectId:int}")]
    public async Task<IActionResult> GetDeploymentHistory(int projectId)
    {
        var results = await _deploymentService.GetDeploymentHistory(projectId);

        return Ok(results.Select(d => new
        {
            d.id,
            d.project_id,
            d.version,
            d.environment,
            d.deployment_id,
            d.commit_hash,
            d.deployed_at,
            d.received_at
        }));
    }
}
