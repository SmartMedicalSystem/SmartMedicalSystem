using Application.DTOs.Auth;
using Application.Interfaces.Services;
using Domain.Identity;
using Domain.Repositories;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IMemberRepo _memberRepo;
    private readonly ITokenService _tokenService;

public AuthService(
    IMemberRepo memberRepo,
    ITokenService tokenService)
    {
        _memberRepo = memberRepo;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(
        RegisterRequestDTO request)
    {
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

        var permissions =
            await _memberRepo.GetPermissionsAsync(
                request.Role);

        var token =
            await _tokenService.CreateTokenAsync(
                user.UserName!,
                user.Email!,
                request.Role,
                permissions);

        var refreshToken =
            _memberRepo.GenerateRefreshToken();

        user.RefreshToken = refreshToken;

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = "User registered successfully",
            AccessToken = token.AccessToken,
            Expiration = token.ExpirationDate,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponseDto> LoginAsync(
        LoginRequestDto request)
    {
        var user =
            await _memberRepo.IsValidUsernameAsync(
                request.UserNameOrEmail)
            ??
            await _memberRepo.IsValidEmailAsync(
                request.UserNameOrEmail);

        if (user is null)
            throw new Exception(
                "Invalid username or email");

        var validUser =
            await _memberRepo.IsValidPasswordAsync(
                request.Password,
                user);

        if (validUser is null)
            throw new Exception(
                "Invalid password");

        var role =
            await _memberRepo.GetRoleAsync(user);

        if (string.IsNullOrWhiteSpace(role))
            throw new Exception(
                "User has no assigned role");

        var permissions =
            await _memberRepo.GetPermissionsAsync(
                role);

        var token =
            await _tokenService.CreateTokenAsync(
                user.UserName!,
                user.Email!,
                role,
                permissions);

        var refreshToken =
            _memberRepo.GenerateRefreshToken();

        user.RefreshToken = refreshToken;

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = "Login successful",
            AccessToken = token.AccessToken,
            Expiration = token.ExpirationDate,
            RefreshToken = refreshToken
        };
    }


}
