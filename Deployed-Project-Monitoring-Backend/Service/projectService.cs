using Data;
using DTOs;
using Microsoft.EntityFrameworkCore;
using Models;
using ProjectInterface;
using Security;
namespace Services;
public class ProjectService : IProjectService
{
	private readonly ApplicationDbContext _context;

	public ProjectService(ApplicationDbContext context)
	{
		_context = context;
	}

    public async Task<ProjectCreationResult?> InsertProject(
    ProjectsInfo projectInfo)
    {
        if (projectInfo == null)
            return null;

        // Generate the original API key.
        var apiKey = ApiKeyGenerator.Generate();

        // Store only its hash.
        var project = new Projects
        {
            project_name = projectInfo.project_name,
            description = projectInfo.description,
            provider = projectInfo.provider,
            deployment_url = projectInfo.deployment_url,
            user_id = projectInfo.user_id,
            api_key_hash = ApiKeyGenerator.Hash(apiKey)
        };

        _context.Projects.Add(project);

        await _context.SaveChangesAsync();

        // Return the original key once, along with the project.
        return new ProjectCreationResult
        {
            Project = project,
            ApiKey = apiKey
        };
    }

    public async Task<List<Projects>> GetProjectsByUserId(int userId)
	{
		return await _context.Projects
			.Where(project => project.user_id == userId)
			.ToListAsync();
	}
}
