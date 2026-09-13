namespace IdentityService.Application.DTOs.Auth;

public class RegisterResponse
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}