namespace IdentityService.Application.Interfaces.Services;

public interface IJwtService
{
   string GenerateToken(long userId, string userName, IList<string> roles);
}