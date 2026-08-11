using Application.DTOs.Auth;
using Application.DTOs.User;
using Domain.Identity;
using System.Security.Cryptography;

namespace Application.Services.Abstraction.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<AuthResponseDto> ForgetPasswordAsync(ForgetPasswordRequestDto request);

        internal Task<AuthResponseDto> CreateAuthResponseAsync(ApplicationUser user, string role, string message);

        Task<ResetPasswordResponseDto> ResetPasswordAsync(NewPasswordRequestDto request);

    }
}