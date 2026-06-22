using Application.DTOs.Auth;
using Application.Interfaces.Services;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly IRoleService _roleService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ITokenService tokenService,
        IRoleService roleService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _roleService = roleService;
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
            await _roleService.AddRoleAsync(user, request.Roles);
        }

        var roles = await _userManager.GetRolesAsync(user);

        var token = await _tokenService.CreateTokenAsync(user.UserName!, user.Email, roles);

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = string.Empty,
            AccessToken = token.AccessToken,
            Expiration = token.ExpirationDate
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

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = string.Empty,
            AccessToken = token.AccessToken,
            Expiration = token.ExpirationDate
        };
    }
}