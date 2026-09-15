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

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost("register-supervisor")]
    public async Task<IActionResult> RegisterSupervisor([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RegisterSupervisorAsync(request, cancellationToken);

        return Ok(result);
    }

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost("register-agent")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RegisterAgentAsync(request, cancellationToken);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.LoginAsync(request, cancellationToken);

        return Ok(result);
    }

    
}