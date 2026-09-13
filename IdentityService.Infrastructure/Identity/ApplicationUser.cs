using Microsoft.AspNetCore.Identity;

namespace IdentityService.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<long>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string NationalNumber { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
}