using Application.DTOs.Auth;
using Application.Interfaces.Services;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ITokenService _tokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }

    // ---------------- REGISTER ----------------
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDTO request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ",
                result.Errors.Select(e => e.Description)));
        }
        // assign roles
        if (request.Roles is not null && request.Roles.Any())
        {
            await AddRoleAsync(user, request.Roles);
        }

        var roles = await _userManager.GetRolesAsync(user);

        var token = await _tokenService.CreateTokenAsync(user.UserName!, user.Email, roles);
        var refreshToken = GenerateRefreshToken();

        return new AuthResponseDto
        { 
            IsSuccess = true,
            Message = string.Empty,
            AccessToken = token.AccessToken,
            Expiration = token.ExpirationDate,
            RefreshToken = refreshToken,
        };
    }

    // ---------------- LOGIN ----------------
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByNameAsync(request.UserNameOrEmail)
                   ?? await _userManager.FindByEmailAsync(request.UserNameOrEmail);

        if (user == null)
            throw new Exception("Invalid username or email");

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordValid)
            throw new Exception("Invalid password");

        var roles = await _userManager.GetRolesAsync(user);

        var token = await _tokenService.CreateTokenAsync(user.UserName!, user.Email, roles);

        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;

        



        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = string.Empty,
            AccessToken = token.AccessToken,
            Expiration = token.ExpirationDate,
            RefreshToken = refreshToken
            
        };

   
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }


     public async Task AddRoleAsync(ApplicationUser user, IEnumerable<string> roleNames)
        {
            if (roleNames == null)
                return;

            foreach (var roleName in roleNames)
            {
                if (string.IsNullOrWhiteSpace(roleName))
                    continue;

                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                }
            }

            await _userManager.AddToRolesAsync(user, roleNames);
        }
}