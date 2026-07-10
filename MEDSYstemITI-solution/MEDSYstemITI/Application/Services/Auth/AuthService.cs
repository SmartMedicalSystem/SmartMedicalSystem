using Application.Common;
using Application.DTOs.Auth;
using Application.Services.Abstraction.Auth;
using Domain.Identity;
using Domain.IRepository;

namespace Application.Services.Auth;

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
            throw new AuthenticationException("Username already exists.");

        if (await _memberRepo.IsValidEmailAsync(request.Email) != null)
            throw new AuthenticationException("Email already exists.");

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
            throw new AuthenticationException(
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
            throw new AuthenticationException("Invalid username or email.");

        var validUser =
            await _memberRepo.IsValidPasswordAsync(
                request.Password,
                user);

        if (validUser is null)
            throw new AuthenticationException("Invalid password.");

        var role =
            await _memberRepo.GetRoleAsync(user);

        if (string.IsNullOrWhiteSpace(role))
            throw new AuthenticationException("User has no assigned role.");

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
            throw new AuthenticationException("Username is missing.");

        if (string.IsNullOrWhiteSpace(user.Email))
            throw new AuthenticationException("Email is missing.");

        var permissions =
            await _memberRepo.GetPermissionsAsync(role);

        var token =
            await _tokenService.CreateTokenAsync(
                user.Id,
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