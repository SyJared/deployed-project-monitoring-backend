using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using UserInterfaces;
using DTOs;
using Data;

namespace Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordHasher<Users> _passwordHasher;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<Users>();
    }

    public async Task<Users?> RegisterAsync(RegisterUser request)
    {
        // Check duplicate email
        var emailExists = await _context.Users
            .AnyAsync(u => u.email == request.email);

        if (emailExists)
            return null;

        // Check duplicate username
        var usernameExists = await _context.Users
            .AnyAsync(u => u.username == request.username);

        if (usernameExists)
            return null;

        var user = new Users
        {
            first_name = request.first_name,
            last_name = request.last_name,
            email = request.email,
            username = request.username
        };

        user.password = _passwordHasher.HashPassword(
            user,
            request.password
        );

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<Users?> Login(LoginUser loginUser)
    {
        if (loginUser == null)
        {
            return null;
        }


        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.username == loginUser.username);


        if (user == null)
        {
            return null;
        }


        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.password,
            loginUser.password
        );

        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return user;
    }

}