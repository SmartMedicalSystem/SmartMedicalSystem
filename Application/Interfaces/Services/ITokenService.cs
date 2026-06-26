using Application.DTOs.Auth;
using System.Collections.Generic;

namespace Application.Interfaces.Services;

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
}
