using Data;
using DTOs;
using Models;
using ProjectInterface;
namespace Services;
public class ProjectService : IProjectService
{
	private readonly ApplicationDbContext _context;

	public ProjectService(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<Projects?> InsertProject(ProjectsInfo projectInfo)
	{
		if (projectInfo == null)
		{
			return null;
		}
		var project = new Projects
		{
			project_name = projectInfo.project_name,
			description = projectInfo.description,
			provider = projectInfo.provider,
			deployment_url = projectInfo.deployment_url,
			user_id = projectInfo.user_id
		};
		_context.Projects.Add(project);
		await _context.SaveChangesAsync();
		return project;
	}
}