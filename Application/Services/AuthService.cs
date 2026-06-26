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

    private async Task<AuthResponseDto> CreateAuthResponseAsync(
        ApplicationUser user,
        string role,
        string message)
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
            _memberRepo.GenerateRefreshToken();

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
}