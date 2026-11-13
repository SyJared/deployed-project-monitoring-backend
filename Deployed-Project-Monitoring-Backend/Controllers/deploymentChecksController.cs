
using DeploymentChecksInterface;
using DTOs;
using Microsoft.AspNetCore.Mvc;


namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonitoringController : ControllerBase
{
    private readonly IDeploymentChecksService _deploymentChecksService;

    public MonitoringController(IDeploymentChecksService monitor)
    {
        _deploymentChecksService = monitor;
    }

    [HttpPost("check/{projectId:int}")]
    public async Task<IActionResult> CheckProject(int projectId)
    {
        var result = await _deploymentChecksService.CheckProject(projectId);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Project not found."
            });
        }

        return Ok(new DeploymentCheckResponse
        {
            id = result.id,
            project_id = result.project_id,
            checked_at = result.checked_at,
            http_status_code = result.http_status_code,
            response_time_ms = result.response_time_ms,
            status = result.status,
            error_message = result.error_message
        });
    }
}
