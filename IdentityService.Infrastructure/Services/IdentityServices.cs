using IdentityService.Application.Constants;
using IdentityService.Application.DTOs.Auth;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Infrastructure.Services;

public class IdentityServices : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;

    public IdentityServices(UserManager<ApplicationUser> userManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByNameAsync(request.UserName);

        if (existingUser is not null)
            throw new Exception("Username already exists.");

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            NationalNumber = request.NationalNumber,
            BirthDate = request.BirthDate
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault()?.Description ?? "Registration failed.";
            throw new Exception(error);
        }

        return new RegisterResponse
        {
            UserId = user.Id,
            UserName = user.UserName!,
            Message = "User registered successfully."
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);

        if (user is null)
            throw new Exception("Invalid username or password.");

        var passwordIsValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordIsValid)
            throw new Exception("Invalid username or password.");

        var roles = await _userManager.GetRolesAsync(user);

        var token = _jwtService.GenerateToken(user.Id, user.UserName!, roles);

        return new LoginResponse
        {
            Message = "Login successful.",
            UserId = user.Id,
            UserName = user.UserName!,
            AccessToken = token
        };
    }

    public async Task<RegisterResponse> CreateCustomerAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            NationalNumber = request.NationalNumber,
            BirthDate = request.BirthDate
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new Exception(result.Errors.FirstOrDefault()?.Description ?? "Customer creation failed.");

        var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.Customer);

        if (!roleResult.Succeeded)
            throw new Exception(roleResult.Errors.FirstOrDefault()?.Description ?? "Failed to assign Customer role.");

        return new RegisterResponse
        {
            Message = "Customer created successfully.",
            UserId = user.Id,
            UserName = user.UserName!
        };
    }
}
