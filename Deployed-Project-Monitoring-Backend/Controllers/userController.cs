
using Microsoft.AspNetCore.Mvc;
using DTOs;
using UserInterfaces;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUser request)
    {
        var user = await _userService.RegisterAsync(request);

        if (user == null)
        {
            return BadRequest(new
            {
                message = "Email or username already exists."
            });
        }

        return StatusCode(201, new
        {
            message = "User registered successfully.",
            user.id,
            user.first_name,
            user.last_name,
            user.email,
            user.username
        });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUser loginUser)
    {
        var user = await _userService.Login(loginUser);
        if (user == null)
        {
            return BadRequest(new
            {
                message = "Invalid username or password."
            });
        }
        return Ok(new
        {
            message = "Login successful.",
            user.id,
            user.first_name,
            user.last_name,
            user.email,
            user.username
        });
    }
}
