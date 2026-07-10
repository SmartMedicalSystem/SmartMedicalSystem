using Application.DTOs.Auth;
using System.Security.Cryptography;

namespace Application.Services.Abstraction.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDTO request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
      
    }
}