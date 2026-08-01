using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User;

public class CreateUserRequestDto
{
    [Required(ErrorMessage = "First name can't be empty")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name must contain only letters")]
    [StringLength(25, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 25 characters")]
    public string FirstName { get; set; } = string.Empty;


    //last name

    [Required(ErrorMessage = "Last name can't be empty")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name must contain only letters")]
    [StringLength(25, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 25 characters")]
    public string LastName { get; set; } = string.Empty;

    //email

    [Required(ErrorMessage = "Email can't be empty")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format.")]

    public string Email { get; set; } = string.Empty;

    //phone number

    [Required(ErrorMessage = "Phone can't be empty")]
    [RegularExpression(@"^[0][1][0125][0-9]{8}$", ErrorMessage = "Invalid  mobile number.")]
    public string? PhoneNumber { get; set; }



    //username

    [Required(ErrorMessage = "Username can't be empty")]
    [StringLength(25, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 25 characters")]
    [RegularExpression(@"^[a-zA-Z0-9_\u0600-\u06FF]+$", 
    ErrorMessage = "Username can contain English letters, numbers, underscores, and Arabic characters.")]
    public string Username { get; set; } = string.Empty;


    //password

    [Required(ErrorMessage = "Password can't be empty")]
    public string Password { get; set; } = string.Empty;


    //Role
    [Required(ErrorMessage = "Role can't be empty")]
    public string Role { get; set; } = string.Empty;

    //PersonId 
    public int PersonId { get; set; }



}
