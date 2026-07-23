using Application.DTOs.LabTechnician;
using Application.DTOs.LabTechProfile;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface ILabTechProfileService
    {
        Task<LabTechProfileReadDto> GetByIdAsync(string id);

        Task<LabTechnicianReadDto> UpdatePublicInfoAsync(int id, LabTechProfileUpdateDto dto);
    }
}