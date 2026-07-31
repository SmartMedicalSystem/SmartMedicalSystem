using Application.DTOs.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Abstraction.Auth;

public interface ITokenService
{
    Task<TokenResponseDto> CreateTokenAsync(
       int applicationUserId,
       int basePersonId,
       string userName,
       string email,
       string role,
       string? photoUrl,
       IEnumerable<string> permissions);

    Task<TokenResponseDto> CreateTokenAsync(
        int applicationUserId,
        int basePersonId,
        string userName,
        string email,
        IList<string> roles);

    Task<TokenResponseDto> GenerateRefreshToken();
}
