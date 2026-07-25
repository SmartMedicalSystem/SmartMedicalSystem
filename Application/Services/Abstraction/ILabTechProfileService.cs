using Application.DTOs.LabTechnician;
using Application.DTOs.Profile;
using Application.DTOs.User;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface IProfileService
    {
        Task<ProfileReadDto> GetByIdAsync(string id);

        Task<ProfileReadDto> UpdatePublicInfoAsync(int id, ProfileUpdateDto dto);

        Task<ProfileReadDto> UpdateUserInfoAsync(int id, UserUpdateDto dto);

        Task<UserReadDto> ChangePasswordAsync(int id, ChangePasswordRequestDto request);
    }

}