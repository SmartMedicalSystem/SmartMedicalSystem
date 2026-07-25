using Application.DTOs.Auth;
using Application.DTOs.User;
using System.Security.Cryptography;

namespace Application.Services.Abstraction.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDto> CreateUserAsync(CreateUserRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<AuthResponseDto> ForgetPasswordAsync(ForgetPasswordRequestDto request);

        Task<ResetPasswordResponseDto> ResetPasswordAsync(NewPasswordRequestDto request);

    }
}