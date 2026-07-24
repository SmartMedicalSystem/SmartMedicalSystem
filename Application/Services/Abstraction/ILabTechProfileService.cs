using Application.DTOs.LabTechnician;
using Application.DTOs.LabTechProfile;
using Application.DTOs.User;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface ILabTechProfileService
    {
        Task<LabTechnicianReadDto> GetByIdAsync(string id);

        Task<LabTechnicianReadDto> UpdatePublicInfoAsync(int id, LabTechProfileUpdateDto dto);

        Task<LabTechnicianReadDto> UpdateUserInfoAsync(int id, UserUpdateDto dto);
    }
}