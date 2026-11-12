using Models;
using DTOs;
namespace UserInterfaces;

public interface IUserService
{
    Task<Users?> RegisterAsync(RegisterUser request);
    Task<Users?> Login(LoginUser loginUser);
}