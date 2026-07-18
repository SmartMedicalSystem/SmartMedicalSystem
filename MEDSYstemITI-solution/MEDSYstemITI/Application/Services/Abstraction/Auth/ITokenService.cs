using Application.DTOs.Auth;
using System.Collections.Generic;

namespace Application.Services.Abstraction.Auth;

public interface ITokenService
{
    Task<TokenResponseDto> CreateTokenAsync(
        string userName,
        string email,
        IList<string> roles);

    Task<TokenResponseDto> CreateTokenAsync(
        string userName,
        string email,
        string role,
        IEnumerable<string> permissions);

    Task<TokenResponseDto> GenerateRefreshToken();
}