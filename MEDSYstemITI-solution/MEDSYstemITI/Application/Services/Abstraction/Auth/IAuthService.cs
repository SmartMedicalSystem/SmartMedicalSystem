using Application.DTOs.Auth;
using System.Security.Cryptography;

namespace Application.Services.Abstraction.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDTO request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<AuthResponseDto> ChangePasswordAsync(ChangePasswordRequestDto request);
        Task<AuthResponseDto> ForgetPasswordAsync(ForgetPasswordRequestDto request);

        Task<AuthResponseDto> ResetPasswordAsync(NewPasswordRequestDto request);
        Task<AuthResponseDto> NewPasswordAsync(NewPasswordRequestDto request);

    }
}