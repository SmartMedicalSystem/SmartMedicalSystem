using Application.DTOs.Auth;
using System.Security.Cryptography;

namespace Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDTO request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
      
    }
}