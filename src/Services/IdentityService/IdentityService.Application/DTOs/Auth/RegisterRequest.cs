using System.ComponentModel.DataAnnotations;
namespace IdentityService.Application.DTOs.Auth;
public class RegisterRequest
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 6, ErrorMessage = "Username must be between 6 and 50 characters.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "First Name is required.")] 
    [StringLength(50, ErrorMessage = "First Name must be maximum 50")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last Name is required.")]
    [StringLength(50, ErrorMessage = "Last Name must be maximum 50")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "National Number is required.")]
    public string NationalNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Birth Date is required.")]
    public DateTime? BirthDate { get; set; }
}