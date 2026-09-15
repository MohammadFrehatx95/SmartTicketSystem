using IdentityService.Application.Constants;
using IdentityService.Application.DTOs.Auth;
using Shared.Application.Exceptions;
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

    public async Task<RegisterResponse> RegisterAgentAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
            throw new ConflictException("Email already exists.");

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            NationalNumber = request.NationalNumber,
            BirthDate = request.BirthDate ?? throw new BadRequestException("Birth Date is required.")
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault()?.Description ?? "Registration failed.";
            throw new BadRequestException(error);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.Agent);

        if (!roleResult.Succeeded)
            throw new BadRequestException(roleResult.Errors.FirstOrDefault()?.Description ?? "Failed to assign Agent role.");

        return new RegisterResponse
        {
            Message = "Agent registered successfully.",
            UserId = user.Id,
            UserName = user.UserName!
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
            throw new UnauthorizedException("Invalid email or password.");

        var passwordIsValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordIsValid)
            throw new UnauthorizedException("Invalid email or password.");

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

    public async Task<RegisterResponse> RegisterSupervisorAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
            throw new ConflictException("Email already exists.");

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            NationalNumber = request.NationalNumber,
            BirthDate = request.BirthDate ?? throw new BadRequestException("Birth Date is required.")
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new BadRequestException(result.Errors.FirstOrDefault()?.Description ?? "Supervisor registration failed.");

        var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.Supervisor);

        if (!roleResult.Succeeded)
            throw new BadRequestException(roleResult.Errors.FirstOrDefault()?.Description ?? "Failed to assign Supervisor role.");

        return new RegisterResponse
        {
            Message = "Supervisor registered successfully.",
            UserId = user.Id,
            UserName = user.UserName!
        };
    }
}
