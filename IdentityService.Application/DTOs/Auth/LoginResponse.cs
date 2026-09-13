namespace IdentityService.Application.DTOs.Auth;

public class LoginResponse
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}