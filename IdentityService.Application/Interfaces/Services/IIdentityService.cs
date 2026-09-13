using IdentityService.Application.DTOs.Auth;
using IdentityService.Application.DTOs.Users;

namespace IdentityService.Application.Interfaces.Services;

public interface IIdentityService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<RegisterResponse> CreateCustomerAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse> GetCustomerByIdAsync(long userId);
}