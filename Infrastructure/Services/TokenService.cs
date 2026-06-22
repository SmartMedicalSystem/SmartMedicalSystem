using Application.DTOs.Auth;
using Application.Interfaces.Services;
using Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{

    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(
            IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }

        public Task<TokenResponseDto> CreateTokenAsync(
            string userName,
            string email,
            IList<string> roles)
        {
            var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.Email, email),

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

            foreach (var role in roles)
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.Role,
                        role));
            }

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        _jwtSettings.Key));

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

            var expirationDate =
                DateTime.UtcNow.AddMinutes(
                    _jwtSettings.ExpireMinutes);

            var token =
                new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: expirationDate,
                    signingCredentials: credentials);

            var jwt =
                new JwtSecurityTokenHandler()
                    .WriteToken(token);

            return Task.FromResult(
                new TokenResponseDto
                {
                    AccessToken = jwt,
                    ExpirationDate = expirationDate
                });
        }
    }
}