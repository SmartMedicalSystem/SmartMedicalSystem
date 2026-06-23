using Application.DTOs.Auth;
using System.Security.Cryptography;

namespace Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDTO request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        public  string GenerateRefreshToken();
        //{
        //    var bytes = new byte[64];
        //    using var rng = RandomNumberGenerator.Create();
        //    rng.GetBytes(bytes);
        //    return Convert.ToBase64String(bytes);
        //}

    }
}