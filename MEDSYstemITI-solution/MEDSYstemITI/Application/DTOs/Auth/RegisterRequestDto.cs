using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

public class RegisterRequestDTO
{
    [Required(ErrorMessage = "First name can't be empty")]
    public string FirstName { get; set; } = string.Empty;


[Required(ErrorMessage = "Last name can't be empty")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email can't be empty")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone can't be empty")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Username can't be empty")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password can't be empty")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role can't be empty")]
    public string Role { get; set; } = string.Empty;
}
