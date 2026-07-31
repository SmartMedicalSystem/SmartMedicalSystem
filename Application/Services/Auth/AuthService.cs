using Application.Common;
using Application.DTOs.Auth;
using Application.DTOs.User;
using Application.Services.Abstraction;
using Application.Services.Abstraction.Auth;
using Domain.Enums;
using Domain.Identity;
using Domain.IRepository;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IMemberRepo _memberRepo;
    private readonly ITokenService _tokenService;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<AuthService> _logger;
    private readonly IUnitOfWork _uow;

    public AuthService(IMemberRepo memberRepo, ITokenService tokenService, IEmailSender emailSender, ILogger<AuthService> logger, IUnitOfWork uow)
    {
        _memberRepo = memberRepo;
        _tokenService = tokenService;
        _emailSender = emailSender;
        _logger = logger;
        _uow = uow;
    }

    public async Task<AuthResponseDto> CreateUserAsync(CreateUserRequestDto request)
    {
        _logger.LogInformation(
            "Admin is attempting to create user {Username} with role {Role}",
            request.Username,
            request.Role);

        if (string.IsNullOrWhiteSpace(request.Role))
        {
            _logger.LogWarning(
                "Registration failed: role is missing for {Username}",
                request.Username);

            throw new ValidationException(
                "Role is required.",
                new[] { new Application.Common.Models.ErrorDetail { Field = "role", Message = "ROLE_REQUIRED" } });
        }

        if (!Enum.TryParse<Roles>(
                request.Role,
                true,
                out var requestedRole))
        {
            _logger.LogWarning(
                "Creating user failed: invalid role {Role}",
                request.Role);

            throw new ValidationException(
                "Invalid role.",
                new[] { new Application.Common.Models.ErrorDetail { Field = "role", Message = "INVALID_ROLE" } });
        }

        if (await _memberRepo.IsValidUsernameAsync(request.Username) != null)
        {
            _logger.LogWarning(
                "Creating user failed: username exists {Username}",
                request.Username);

            throw new ConflictException(
                "Username already exists.",
                "USERNAME_ALREADY_EXISTS");
        }

        if (await _memberRepo.IsValidEmailAsync(request.Email) != null)
        {
            _logger.LogWarning(
                "Creating user failed: email exists {Email}",
                request.Email);

            throw new ConflictException(
                "Email already exists.",
                "EMAIL_ALREADY_EXISTS");
        }

        var user = new ApplicationUser
        {
            FullName = $"{request.FirstName} {request.LastName}",
            UserName = request.Username,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            EmailConfirmed = true,
            PersonId = request.PersonId,
        };


        var result = await _memberRepo.RegisterAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            var errorDetails = result.Errors.Select(e => new Application.Common.Models.ErrorDetail { Message = e.Description });

            _logger.LogWarning(
                "Registration failed for {Username}: {Errors}",
                request.Username,
                string.Join(", ", result.Errors.Select(e => e.Description)));

            throw new ValidationException(
                "Password validation failed.",
                errorDetails);
        }

        var roleAdded = await _memberRepo.AddRoleAsync(
            user,
            requestedRole.ToString());

        if (!roleAdded)
        {
            _logger.LogError(
                "User {Username} was created but role {Role} could not be assigned",
                user.UserName,
                requestedRole);

            throw new InternalServerException(
                "User was created but role assignment failed.",
                "ROLE_ASSIGNMENT_FAILED");
        }

        _logger.LogInformation(
            "User registered successfully {Username} with role {Role}",
            user.UserName,
            requestedRole);

        return await CreateAuthResponseAsync(
            user,
            requestedRole.ToString(),
            "User registered successfully");
    }

    //login method
    public async Task<AuthResponseDto> LoginAsync( LoginRequestDto request)
    {
        _logger.LogInformation(
            "Login attempt for {UserOrEmail}",
            request.UserNameOrEmail);

        var user =
            await _memberRepo.FindByUsernameOrEmailAsync(
                request.UserNameOrEmail);

        if (user is null)
        {
            _logger.LogWarning(
                "Login failed: user not found {UserOrEmail}",
                request.UserNameOrEmail);

            throw new UnauthorizedException(
                "Invalid username or password.",
                "INVALID_CREDENTIALS");
        }

        var validUser =
            await _memberRepo.IsValidPasswordAsync(
                request.Password,
                user);

        if (validUser is null)
        {
            _logger.LogWarning(
                "Login failed: invalid password for {User}",
                request.UserNameOrEmail);

            throw new UnauthorizedException(
                "Invalid username or password.",
                "INVALID_CREDENTIALS");
        }

        var role =
            await _memberRepo.GetRoleAsync(user);

        if (string.IsNullOrWhiteSpace(role))
        {
            _logger.LogWarning(
                "Login failed: no role for user {User}",
                user.UserName);

            throw new UnauthorizedException(
                "User has no assigned role.",
                "ROLE_REQUIRED");
        }

        _logger.LogInformation(
            "Login successful for user {User}",
            user.UserName);

        return await CreateAuthResponseAsync(
            user,
            role,
            "Login successful");
    }

    private async Task<AuthResponseDto> CreateAuthResponseAsync(ApplicationUser user,string role, string message)
    {
        if (string.IsNullOrWhiteSpace(user.UserName))
        {
            throw new InternalServerException(
                "Username is missing.",
                "USERNAME_MISSING");
        }


        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new InternalServerException(
                "Email is missing.",
                "EMAIL_MISSING");
        }

        var permissions =
            await _memberRepo.GetPermissionsAsync(role);


        var PersonId = user.PersonId;
        var person = await _uow.PersonGeneric.FindByIdAsync(PersonId);

        var PhotoUrl = person?.PhotoUrl ?? string.Empty;
        var token =
            await _tokenService.CreateTokenAsync
            (
                user.Id,
                user.PersonId,
                user.UserName,
                user.Email,
                role,
                PhotoUrl,
                permissions
             );

        var refreshTokenResponse =
            await _tokenService.GenerateRefreshToken();

        user.RefreshToken =
            refreshTokenResponse.AccessToken;

        user.RefreshTokenExpiryTime =
            refreshTokenResponse.ExpirationDate;

        await _memberRepo.UpdateAsync(user);

        _logger.LogInformation(
            "Generated tokens for user {User}. Expires at {Expiry}",
            user.UserName,
            token.ExpirationDate);

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = message,
            AccessToken = token.AccessToken,
            Expiration = token.ExpirationDate,
            RefreshToken = user.RefreshToken
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(
        RefreshTokenRequestDto request)
    {
        _logger.LogInformation(
            "Refresh token attempt");

        var user =
            await _memberRepo.GetByRefreshTokenAsync(
                request.RefreshToken);

        if (user is null)
        {
            _logger.LogWarning(
                "Refresh token failed: invalid token");

            throw new UnauthorizedException(
                "Invalid refresh token.",
                "INVALID_REFRESH_TOKEN");
        }

        if (string.IsNullOrWhiteSpace(user.RefreshToken))
        {
            throw new UnauthorizedException(
                "Refresh token is missing.",
                "INVALID_REFRESH_TOKEN");
        }

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            _logger.LogWarning(
                "Refresh token expired for user {User}",
                user.UserName);

            throw new UnauthorizedException(
                "Refresh token has expired.",
                "REFRESH_TOKEN_EXPIRED");
        }

        var role =
            await _memberRepo.GetRoleAsync(user);

        if (string.IsNullOrWhiteSpace(role))
        {
            throw new UnauthorizedException(
                "User has no assigned role.",
                "ROLE_REQUIRED");
        }

        _logger.LogInformation(
            "Refresh token succeeded for user {User}",
            user.UserName);

        return await CreateAuthResponseAsync(
            user,
            role,
            "Token refreshed successfully");
    }


    public async Task<AuthResponseDto> ForgetPasswordAsync(ForgetPasswordRequestDto request)
    {
        _logger.LogInformation("Forget password requested for {Email}", request.Email);
        var token = await _memberRepo.GeneratePasswordResetTokenAsync(request.Email);

        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "If the email exists, password reset instructions have been sent."
            };
        }

        var encodedToken = WebUtility.UrlEncode(token);

        var resetLink =
            $"http://localhost:4200/auth/new-password?email={request.Email}";

        string logoImage = "https://smartmedicalsystem.runasp.net/uploads/doctor-logo.png";

        await _emailSender.SendEmailAsync(
            new Application.DTOs.Email.Message(
                new List<string> { request.Email },
                "Reset Your Password - MEDSystem",
                $"""
<!DOCTYPE html>
<html>

<head>
<meta charset="UTF-8">
</head>

<body style="
margin:0;
padding:0;
background:#eef6ff;
font-family:'Segoe UI',Arial,sans-serif;
">


<table width="100%"
cellpadding="0"
cellspacing="0"
style="padding:25px 10px;">


<tr>
<td align="center">


<table width="520"
cellpadding="0"
cellspacing="0"
style="
background:#ffffff;
border-radius:16px;
overflow:hidden;
box-shadow:0 8px 25px rgba(0,0,0,.10);
">


<!-- HEADER -->

<tr>
<td align="center"
style="
background:linear-gradient(135deg,#2563eb,#06b6d4);
padding:30px 15px;
">


<img
src="{logoImage}"
width="85"
alt="MEDSystem"
style="
background:white;
padding:10px;
border-radius:50%;
display:block;
">


<h1
style="
color:white;
font-size:26px;
margin:15px 0 5px;
">

MEDSystem

</h1>


<p
style="
color:#dbeafe;
font-size:13px;
margin:0;
">

Smart Healthcare Management System

</p>


</td>
</tr>



<!-- CONTENT -->


<tr>

<td
style="
padding:30px 35px;
color:#334155;
">


<h2
style="
text-align:center;
font-size:23px;
color:#0f172a;
margin:0 0 20px;
">

Reset Your Password

</h2>



<p
style="
font-size:14px;
line-height:1.7;
margin:0;
">

Hello,

<br><br>

We received a request to reset your password.

Your account security is important to us.

Click the button below to create a new password.

</p>




<!-- BUTTON -->

<table width="100%">

<tr>

<td align="center"
style="padding:25px 0;">


<a href="{resetLink}"

style="
background:#2563eb;
color:white;
text-decoration:none;
padding:13px 32px;
border-radius:40px;
font-size:15px;
font-weight:bold;
display:inline-block;
">

Reset Password

</a>


</td>

</tr>

</table>




<!-- LINK BOX -->

<table width="100%"
style="
background:#f8fafc;
border-radius:10px;
">

<tr>

<td style="padding:15px;">


<p
style="
font-size:12px;
color:#64748b;
margin:0 0 8px;
">

If the button doesn't work:

</p>


<a href="{resetLink}"

style="
font-size:12px;
color:#2563eb;
word-break:break-all;
">

{resetLink}

</a>


</td>

</tr>

</table>





<!-- WARNING -->


<table width="100%"
style="
margin-top:20px;
background:#fff7ed;
border-left:4px solid #f97316;
border-radius:6px;
">


<tr>

<td
style="
padding:12px;
color:#9a3412;
font-size:12px;
">

⚠️ If you didn't request this reset, ignore this email.

</td>

</tr>


</table>



</td>

</tr>




<!-- FOOTER -->


<tr>

<td
align="center"
style="
background:#f8fafc;
padding:18px;
">


<p
style="
margin:0;
color:#94a3b8;
font-size:11px;
">

© 2026 MEDSystem. All rights reserved.

</p>


</td>

</tr>


</table>


</td>
</tr>


</table>


</body>

</html>

"""
            )
        );

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = "If the email exists, password reset instructions have been sent."
        };
    }

    public async Task<ResetPasswordResponseDto> ResetPasswordAsync(
        NewPasswordRequestDto request)
    {
        _logger.LogInformation(
            "Reset password attempt for {Email}",
            request.Email);

        var user =
            await _memberRepo.FindByUsernameOrEmailAsync(
                request.Email);

        if (user is null)
        {
            throw new NotFoundException(
                "User not found.",
                "USER_NOT_FOUND");
        }

        var result =
            await _memberRepo.ResetPasswordAsync(
                user,
                request.NewPassword);

        if (!result.Succeeded)
        {
            var errorDetails = result.Errors.Select(e => new Application.Common.Models.ErrorDetail { Message = e.Description });

            _logger.LogWarning(
                "Reset password failed for {Email}",
                request.Email);

            throw new ValidationException(
                "Password validation failed.",
                errorDetails);
        }

        return new ResetPasswordResponseDto
        {
            isSuccess = true,
            message = "Password has been reset successfully."
        };
    }

    private static string BuildPasswordResetEmail(
        string resetLink)
    {
        return $"""
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
            <title>Reset Password</title>
        </head>

        <body style="
            margin:0;
            padding:0;
            background:#f1f5f9;
            font-family:Arial, sans-serif;
        ">

            <table width="100%" cellpadding="0" cellspacing="0">
                <tr>
                    <td align="center" style="padding:40px 15px;">

                        <table width="600" cellpadding="0" cellspacing="0"
                               style="
                                   max-width:600px;
                                   width:100%;
                                   background:white;
                                   border-radius:12px;
                                   overflow:hidden;
                               ">

                            <tr>
                                <td style="padding:35px;">

                                    <h2 style="
                                        text-align:center;
                                        font-size:23px;
                                        color:#0f172a;
                                        margin:0 0 20px;
                                    ">
                                        Reset Your Password
                                    </h2>

                                    <p style="
                                        font-size:14px;
                                        line-height:1.7;
                                        margin:0;
                                    ">
                                        Hello,
                                        <br><br>

                                        We received a request to reset your password.

                                        Your account security is important to us.

                                        Click the button below to create a new password.
                                    </p>

                                    <table width="100%">
                                        <tr>
                                            <td align="center" style="padding:25px 0;">

                                                <a href="{resetLink}"
                                                   style="
                                                       background:#2563eb;
                                                       color:white;
                                                       text-decoration:none;
                                                       padding:13px 32px;
                                                       border-radius:40px;
                                                       font-size:15px;
                                                       font-weight:bold;
                                                       display:inline-block;
                                                   ">
                                                    Reset Password
                                                </a>

                                            </td>
                                        </tr>
                                    </table>

                                    <table width="100%" style="
                                        background:#f8fafc;
                                        border-radius:10px;
                                    ">
                                        <tr>
                                            <td style="padding:15px;">

                                                <p style="
                                                    font-size:12px;
                                                    color:#64748b;
                                                    margin:0 0 8px;
                                                ">
                                                    If the button doesn't work:
                                                </p>

                                                <a href="{resetLink}"
                                                   style="
                                                       font-size:12px;
                                                       color:#2563eb;
                                                       word-break:break-all;
                                                   ">
                                                    {resetLink}
                                                </a>

                                            </td>
                                        </tr>
                                    </table>

                                    <p style="
                                        font-size:12px;
                                        color:#64748b;
                                        margin-top:25px;
                                    ">
                                        If you didn't request this password reset,
                                        please ignore this email.
                                    </p>

                                </td>
                            </tr>

                            <tr>
                                <td align="center" style="
                                    background:#f8fafc;
                                    padding:18px;
                                ">

                                    <p style="
                                        margin:0;
                                        color:#94a3b8;
                                        font-size:11px;
                                    ">
                                        © 2026 MEDSystem. All rights reserved.
                                    </p>

                                </td>
                            </tr>

                        </table>

                    </td>
                </tr>
            </table>

        </body>
        </html>
        """;
    }

   
}
