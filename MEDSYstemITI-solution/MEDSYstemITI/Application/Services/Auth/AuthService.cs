using Application.DTOs.Auth;
using Application.Services.Abstraction;
using Application.Services.Abstraction.Auth;
using Domain.Identity;
using Domain.IRepository;
using System.Reflection.Metadata;
using System.Net;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IMemberRepo _memberRepo;
    private readonly ITokenService _tokenService;
    private readonly IEmailSender _emailSender;
    public AuthService(IMemberRepo memberRepo,ITokenService tokenService, IEmailSender emailSender)
    {
        _memberRepo = memberRepo;
        _tokenService = tokenService;
        _emailSender = emailSender;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDTO request)
    {
        if (await _memberRepo.IsValidUsernameAsync(request.Username) != null)
            throw new Exception("Username already exists.");

        if (await _memberRepo.IsValidEmailAsync(request.Email) != null)
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
            throw new Exception(
                string.Join(", ",
                result.Errors.Select(e => e.Description)));
        }

        await _memberRepo.AddRoleAsync(
            user,
            request.Role);

        return await CreateAuthResponseAsync(
            user,
            request.Role,
            "User registered successfully");
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user =
            await _memberRepo.FindByUsernameOrEmailAsync(
                request.UserNameOrEmail);

        if (user is null)
            throw new Exception("Invalid username or email.");

        var validUser =
            await _memberRepo.IsValidPasswordAsync(
                request.Password,
                user);

        if (validUser is null)
            throw new Exception("Invalid password.");

        var role =
            await _memberRepo.GetRoleAsync(user);

        if (string.IsNullOrWhiteSpace(role))
            throw new Exception("User has no assigned role.");

        return await CreateAuthResponseAsync(
            user,
            role,
            "Login successful");
    }

    private async Task<AuthResponseDto> CreateAuthResponseAsync(ApplicationUser user, string role, string message)
    {
        if (string.IsNullOrWhiteSpace(user.UserName))
            throw new Exception("Username is missing.");

        if (string.IsNullOrWhiteSpace(user.Email))
            throw new Exception("Email is missing.");

        var permissions =
            await _memberRepo.GetPermissionsAsync(role);


        var token =
            await _tokenService.CreateTokenAsync(
                user.UserName,
                user.Email,
                role,
                permissions);

        var refreshToken =
            _tokenService.GenerateRefreshToken().ToString();

        user.RefreshToken = refreshToken;

        await _memberRepo.UpdateAsync(user);

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
        var user = await _memberRepo.GetByRefreshTokenAsync(request.RefreshToken);

        if (user is null)
            throw new Exception("Invalid refresh token.");

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

        return await CreateAuthResponseAsync(
            user,
            role,
            "Token refreshed successfully");
    }

    public async Task<AuthResponseDto> ChangePasswordAsync(ChangePasswordRequestDto request)
    {
        var user = await _memberRepo.FindByUsernameOrEmailAsync(request.Email);

        if (user is null)
            throw new Exception("User not found.");

        var valid = await _memberRepo.IsValidPasswordAsync(request.CurrentPassword, user);

        if (valid is null)
            throw new Exception("Current password is incorrect.");

        var result = await _memberRepo.ChangePasswordAsync(user,request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            throw new Exception(
                string.Join(", ",
                    result.Errors.Select(e => e.Description)));
        }

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = "Password changed successfully."
        };
    }

    public async Task<AuthResponseDto> ForgetPasswordAsync(ForgetPasswordRequestDto request)
    {
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
            throw new Exception(
                string.Join(", ",
                    result.Errors.Select(e => e.Description)));
        }

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = "Password has been reset successfully."
        };
    }

}