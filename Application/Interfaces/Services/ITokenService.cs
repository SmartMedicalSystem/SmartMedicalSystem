using Application.DTOs.Auth;

namespace Application.Interfaces.Services


{
    public interface ITokenService
    {
        Task<TokenResponseDto> CreateTokenAsync(
            string userName,
            string email,
            IList<string> roles);
    }
}