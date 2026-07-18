using Application.DTOs.Auth;
using Application.Services.Abstraction;
using Application.Services.Abstraction.Auth;
using Domain.Identity;
using Domain.IRepository;
using System.Reflection.Metadata;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IMemberRepo _memberRepo;
    private readonly ITokenService _tokenService;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IMemberRepo memberRepo, ITokenService tokenService, IEmailSender emailSender, ILogger<AuthService> logger)
    {
        _memberRepo = memberRepo;
        _tokenService = tokenService;
        _emailSender = emailSender;
        _logger = logger;
    }

    public Task<AuthResponseDto> NewPasswordAsync(NewPasswordRequestDto request)
    {
        return ResetPasswordAsync(request);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDTO request)
    {
        _logger.LogInformation("Register attempt for username {Username} email {Email}", request.Username, request.Email);
        if (await _memberRepo.IsValidUsernameAsync(request.Username) != null)
        {
            _logger.LogWarning("Registration failed: username exists {Username}", request.Username);
        }
            throw new Exception("Username already exists.");

        if (await _memberRepo.IsValidEmailAsync(request.Email) != null)
        {
            _logger.LogWarning("Registration failed: email exists {Email}", request.Email);
        }
            throw new Exception("Email already exists.");

        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            EmailConfirmed = true
        };

        var result = await _memberRepo.RegisterAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Registration failed for {Username}: {Errors}", request.Username, errors);
            throw new Exception(errors);
        }

        await _memberRepo.AddRoleAsync(user, request.Role);

        _logger.LogInformation("User registered {Username} with role {Role}", user.UserName, request.Role);

        return await CreateAuthResponseAsync(user, request.Role, "User registered successfully");
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        _logger.LogInformation("Login attempt for {UserOrEmail}", request.UserNameOrEmail);
        var user =
            await _memberRepo.FindByUsernameOrEmailAsync(
                request.UserNameOrEmail);

        if (user is null)
        {
            _logger.LogWarning("Login failed: user not found {UserOrEmail}", request.UserNameOrEmail);
            throw new Exception("Invalid username or email.");
        }

        var validUser =
            await _memberRepo.IsValidPasswordAsync(
                request.Password,
                user);

        if (validUser is null)
        {
            _logger.LogWarning("Login failed: invalid password for {User}", request.UserNameOrEmail);
            throw new Exception("Invalid password.");
        }

        var role =
            await _memberRepo.GetRoleAsync(user);

        if (string.IsNullOrWhiteSpace(role))
        {
            _logger.LogWarning("Login failed: no role for user {User}", user.UserName);
            throw new Exception("User has no assigned role.");
        }

        _logger.LogInformation("Login successful for user {User}", user.UserName);

        return await CreateAuthResponseAsync(user, role, "Login successful");
    }

    private async Task<AuthResponseDto> CreateAuthResponseAsync(ApplicationUser user, string role, string message)
    {
        if (string.IsNullOrWhiteSpace(user.UserName))
            throw new Exception("Username is missing.");

        if (string.IsNullOrWhiteSpace(user.Email))
            throw new Exception("Email is missing.");

        var permissions = await _memberRepo.GetPermissionsAsync(role);


        var token = await _tokenService.CreateTokenAsync(user.UserName, user.Email, role, permissions);

        var refreshToken =
            _tokenService.GenerateRefreshToken().ToString();

        user.RefreshToken = refreshToken;

        await _memberRepo.UpdateAsync(user);

        _logger.LogInformation("Generated tokens for user {User}. Expires at {Expiry}", user.UserName, token.ExpirationDate);

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = message,
            AccessToken = token.AccessToken,
            Expiration = token.ExpirationDate,
            RefreshToken = refreshToken
        };
    }
    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        _logger.LogInformation("Refresh token attempt");
        var user = await _memberRepo.GetByRefreshTokenAsync(request.RefreshToken);

        if (user is null)
        {
            _logger.LogWarning("Refresh token failed: invalid token");
            throw new Exception("Invalid refresh token.");
        }

        if (string.IsNullOrWhiteSpace(user.RefreshToken))
            throw new Exception("Refresh token is missing.");

        // If you store an expiration date, validate it here.
        // Example:
        // if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        //     throw new Exception("Refresh token has expired.");

        var role = await _memberRepo.GetRoleAsync(user);

        if (string.IsNullOrWhiteSpace(role))
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "User has no assigned role."
            };

        _logger.LogInformation("Refresh token succeeded for user {User}", user.UserName);

        return await CreateAuthResponseAsync(user, role, "Token refreshed successfully");
    }

    public async Task<AuthResponseDto> ChangePasswordAsync(ChangePasswordRequestDto request)
    {
        _logger.LogInformation("Change password attempt for {Email}", request.Email);
        var user = await _memberRepo.FindByUsernameOrEmailAsync(request.Email);

        if (user is null)
            throw new Exception("User not found.");

        var valid = await _memberRepo.IsValidPasswordAsync(request.CurrentPassword, user);

        if (valid is null)
        {
            _logger.LogWarning("Change password failed: incorrect current password for {Email}", request.Email);
            throw new Exception("Current password is incorrect.");
        }

        var result = await _memberRepo.ChangePasswordAsync(user,request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Change password failed for {Email}: {Errors}", request.Email, errors);
            throw new Exception(errors);
        }

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = "Password changed successfully."
        };
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
            $"https://localhost:4200/reset-password?email={request.Email}&token={encodedToken}";

        await _emailSender.SendEmailAsync( new Application.DTOs.Email.Message(new List<string> 
        { request.Email },
                "Reset Password",
                $"""
            You requested a password reset.

            Click the following link:

            {resetLink}

            If you didn't request this, ignore this email.
            """));

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = "If the email exists, password reset instructions have been sent."
        };
    }

    public async Task<AuthResponseDto> ResetPasswordAsync(NewPasswordRequestDto request)
    {
        _logger.LogInformation("Reset password attempt for {Email}", request.Email);
        var user = await _memberRepo.FindByUsernameOrEmailAsync(request.Email);

        if (user is null)
            throw new Exception("User not found.");

        var decodedToken = WebUtility.UrlDecode(request.Token);

        var result = await _memberRepo.ResetPasswordAsync(
            request.Email,
            decodedToken,
            request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Reset password failed for {Email}: {Errors}", request.Email, errors);
            throw new Exception(errors);
        }

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = "Password has been reset successfully."
        };
    }

}