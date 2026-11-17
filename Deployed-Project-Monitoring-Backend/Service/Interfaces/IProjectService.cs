using DTOs;
using Models;
namespace ProjectInterface;
public interface IProjectService
{
    Task<ProjectCreationResult?> InsertProject(ProjectsInfo projectInfo);
    Task<List<Projects>> GetProjectsByUserId(int userId);
}
