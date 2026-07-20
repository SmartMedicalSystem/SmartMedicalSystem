using Application.DTOs.Auth;
using Application.Services.Abstraction.Auth;
using Domain.Constants;
using Infrastructure.Context.Configurations.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    // =========================================================
    // Create Token With Role And Permissions
    // =========================================================

    public Task<TokenResponseDto> CreateTokenAsync(
        int applicationUserId,
        int basePersonId,
        string userName,
        string email,
        string role,
        IEnumerable<string> permissions)
    {
        var claims = new List<Claim>
        {
            // ApplicationUser.Id
            new(
                ClaimTypes.NameIdentifier,
                applicationUserId.ToString()),

            // BasePerson.Id
            new(
                CustomClaimTypes.BasePersonId,
                basePersonId.ToString()),

            // Username
            new(
                ClaimTypes.Name,
                userName),

            // Email
            new(
                ClaimTypes.Email,
                email),

            // Role
            new(
                ClaimTypes.Role,
                role),

            // JWT Standard Claims
            new(
                JwtRegisteredClaimNames.Sub,
                applicationUserId.ToString()),

            new(
                JwtRegisteredClaimNames.Email,
                email),

            new(
                JwtRegisteredClaimNames.UniqueName,
                userName),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        // Add Permissions
        claims.AddRange(
            permissions.Select(permission =>
                new Claim(
                    CustomClaimTypes.Permission,
                    permission)));

        var token = GenerateJwtToken(claims);

        return Task.FromResult(token);
    }


    // =========================================================
    // Create Token With Multiple Roles
    // =========================================================

    public Task<TokenResponseDto> CreateTokenAsync(
        int applicationUserId,
        int basePersonId,
        string userName,
        string email,
        IList<string> roles)
    {
        var claims = new List<Claim>
        {
            // ApplicationUser.Id
            new(
                ClaimTypes.NameIdentifier,
                applicationUserId.ToString()),

            // BasePerson.Id
            new(
                CustomClaimTypes.BasePersonId,
                basePersonId.ToString()),

            // Username
            new(
                ClaimTypes.Name,
                userName),

            // Email
            new(
                ClaimTypes.Email,
                email),

            // JWT Standard Claims
            new(
                JwtRegisteredClaimNames.Sub,
                applicationUserId.ToString()),

            new(
                JwtRegisteredClaimNames.Email,
                email),

            new(
                JwtRegisteredClaimNames.UniqueName,
                userName),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        // Add Roles
        foreach (var role in roles)
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role));
        }

        var token = GenerateJwtToken(claims);

        return Task.FromResult(token);
    }


    // =========================================================
    // Generate JWT
    // =========================================================

    private TokenResponseDto GenerateJwtToken(
        IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _jwtSettings.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expirationDate =
            DateTime.UtcNow.AddMinutes(
                _jwtSettings.ExpireMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expirationDate,
            signingCredentials: credentials);

        var jwt =
            new JwtSecurityTokenHandler()
                .WriteToken(token);

        return new TokenResponseDto
        {
            AccessToken = jwt,
            ExpirationDate = expirationDate
        };
    }


    // =========================================================
    // Generate Refresh Token
    // =========================================================

    public Task<TokenResponseDto> GenerateRefreshToken()
    {
        return Task.FromResult(new TokenResponseDto
        {
            AccessToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()), 
        ExpirationDate = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes)
        }
        ); 
    }

}