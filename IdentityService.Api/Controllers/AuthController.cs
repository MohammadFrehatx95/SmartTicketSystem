using IdentityService.Application.Constants;
using IdentityService.Application.DTOs.Auth;
using IdentityService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RegisterAsync(request, cancellationToken);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.LoginAsync(request, cancellationToken);

        return Ok(result);
    }

    [Authorize(Roles = $"{RoleNames.Agent},{RoleNames.Admin}")]
    [HttpPost("customers")]
    public async Task<IActionResult> CreateCustomer([FromBody] RegisterRequest request)
    {
        var response = await _identityService.CreateCustomerAsync(request);

        return Ok(response);
    }

    [Authorize(Roles = RoleNames.Agent + "," + RoleNames.Admin)]
    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUserById(long id)
    {
        var response = await _identityService.GetCustomerByIdAsync(id);

        return Ok(response);
    }
}