using Application.DTOs.Auth;
using Application.Services.Abstraction;
using Application.Services.Abstraction.Auth;
using Domain.Identity;
using Domain.IRepository;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IMemberRepo _memberRepo;
    private readonly ITokenService _tokenService;

    public AuthService(IMemberRepo memberRepo,ITokenService tokenService)
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
        var user = await _memberRepo.FindByUsernameOrEmailAsync(request.CurrentPassword);

        if (user is null)
            throw new Exception("User not found.");

        var changedUser = await _memberRepo.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        return await CreateAuthResponseAsync(
            changedUser,
            await _memberRepo.GetRoleAsync(changedUser),
            "Password changed successfully");
    }

    public Task<AuthResponseDto> ForgetPasswordAsync(ForgetPasswordRequestDto request)
    {
        throw new NotImplementedException();
    }


}