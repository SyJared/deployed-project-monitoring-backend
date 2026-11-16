
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
    [HttpPost("check-all/{userId:int}")]
    public async Task<IActionResult> CheckAllProjects(int userId)
    {
        
        var results = await _deploymentChecksService.CheckAllProjects(userId);

        return Ok(results?.Select(r => new DeploymentCheckResponse
        {
            id = r.id,
            project_id = r.project_id,
            checked_at = r.checked_at,
            http_status_code = r.http_status_code,
            response_time_ms = r.response_time_ms,
            status = r.status,
            error_message = r.error_message
        }));
    }
    [HttpGet("uptime/{projectId:int}")]
    public async Task<IActionResult> GetUptime(
    int projectId,
    [FromQuery] int days = 30)
    {
        if (days <= 0)
            return BadRequest("Days must be greater than zero.");

        var result = await _deploymentChecksService.GetUptime(projectId, days);

        return Ok(result);
    }
    [HttpGet("downtime/{projectId:int}")]
    public async Task<IActionResult> GetDowntimeHistory(int projectId)
    {
        var results = await _deploymentChecksService.GetProjectDowntimeHistory(projectId);

        return Ok(results.Select(d => new
        {
            d.id,
            d.project_id,
            d.started_at,
            d.resolved_at,
            status = d.status.ToString()
        }));
    }
    [HttpGet("downtime/{projectId:int}/ongoing")]
    public async Task<IActionResult> GetOngoingDowntime(int projectId)
    {
        var result = await _deploymentChecksService.GetOngoingDowntime(projectId);

        if (result == null)
            return NotFound(new
            {
                message = "No ongoing downtime found."
            });

        return Ok(new
        {
            result.id,
            result.project_id,
            result.started_at,
            result.resolved_at,
            status = result.status.ToString()
        });
    }
}
