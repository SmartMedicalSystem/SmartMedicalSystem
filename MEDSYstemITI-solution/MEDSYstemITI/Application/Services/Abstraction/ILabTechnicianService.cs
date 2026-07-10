using Application.DTOs.LabTechnician;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface ILabTechnicianService
    {
        Task<LabTechnicianReadDto> CreateAsync(LabTechnicianCreateDto dto);
        Task<LabTechnicianReadDto> UpdateAsync(int id, LabTechnicianUpdateDto dto);
        Task<LabTechnicianReadDto> GetByIdAsync(int id);
        Task<PaginatedResult<LabTechnicianReadDto>> GetAllAsync(PaginationParams pagination);
        Task DeleteAsync(int id);
    }
}
