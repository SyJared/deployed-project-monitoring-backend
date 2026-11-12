using DTOs;

using Microsoft.AspNetCore.Mvc;
using ProjectInterface;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]

public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }
    [HttpPost("insert")]
    public async Task<IActionResult> InsertProject([FromBody] ProjectsInfo projectInfo)
    {
        var project = await _projectService.InsertProject(projectInfo);
        if (project == null)
        {
            return BadRequest(new
            {
                message = "Failed to insert project."
            });
        }
        return StatusCode(201, new
        {
            message = "Project inserted successfully.",
            project.id,
            project.project_name,
            project.description,
            project.deployment_url,
            project.provider,
            project.user_id
        });
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetProjectsByUserId([FromRoute] int userId)
    {
        var projects = await _projectService.GetProjectsByUserId(userId);
        return Ok(projects.Select(project => new
        {
            project.id,
            project.project_name,
            project.description,
            project.deployment_url,
            project.provider,
            project.user_id,
            project.created_at,
            project.last_checked_at,
            project.status
        }));
    }
}
