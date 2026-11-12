using DTOs;
using Models;
namespace ProjectInterface;
public interface IProjectService
{
    Task<Projects?> InsertProject(ProjectsInfo projectInfo);
    Task<List<Projects>> GetProjectsByUserId(int userId);
}
