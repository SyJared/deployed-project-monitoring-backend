using DTOs;
using Models;
namespace ProjectInterface;
public interface IProjectService
{
    Task<Projects?> InsertProject(ProjectsInfo projectInfo);
    
}