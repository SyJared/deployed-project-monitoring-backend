
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
        [FromHeader(Name = "X-API-Key")] string apiKey,
        [FromBody] DeploymentReportRequest request)
    {
        var result = await _deploymentService.ReportDeployment(
            apiKey,
            request
        );

        if (result == null)
        {
            return Unauthorized(new
            {
                message = "Invalid or missing API key."
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
