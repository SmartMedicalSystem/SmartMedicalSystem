using Application.DTOs.LabTechProfile;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface ILabTechProfileService
    {
        Task<LabTechProfileReadDto> CreateAsync(LabTechProfileCreateDto dto);
        Task<LabTechProfileReadDto> GetByIdAsync(int id);
        Task<LabTechProfileReadDto> GetByUserIdAsync(int userId);
        Task<PaginatedResult<LabTechProfileReadDto>> GetAllPaginatedAsync(PaginationParams pagination);

        /// <summary>Self-service: technician saves Personal + Contact Information.</summary>
        Task<LabTechProfileReadDto> UpdateOwnProfileAsync(int userId, LabTechProfileUpdateDto dto);

        /// <summary>Self-service: technician updates their avatar.</summary>
        Task<LabTechProfileReadDto> UpdateProfilePictureAsync(int userId, LabTechProfilePictureDto dto);

        /// <summary>Admin-only: updates Professional Information section.</summary>
        Task<LabTechProfileReadDto> UpdateProfessionalInfoAsync(int id, LabTechProfileAdminUpdateDto dto);

        /// <summary>Admin-only: activates / deactivates / sets on-leave.</summary>
        Task<LabTechProfileReadDto> UpdateStatusAsync(int id, LabTechProfileStatusDto dto);
    }
}