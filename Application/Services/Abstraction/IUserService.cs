using Application.DTOs.Auth;
using Application.DTOs.User;

namespace Application.Services.Abstraction
{
    public interface IUserService
    {
        int? UserId { get; }
        int? BasePersonId { get; }

        Task<AuthResponseDto> CreateUserAsync(CreateUserRequestDto request);
    }
}
