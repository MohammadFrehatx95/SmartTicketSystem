using IdentityService.Application.DTOs.Auth;
using IdentityService.Application.DTOs.Users;

namespace IdentityService.Application.Interfaces.Services;

public interface IIdentityService
{
    Task<RegisterResponse> RegisterAgentAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<RegisterResponse> RegisterSupervisorAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}