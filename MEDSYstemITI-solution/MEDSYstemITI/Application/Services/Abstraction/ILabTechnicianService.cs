using Application.DTOs.LabTechnician;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface ILabTechnicianService
    {
        Task<LabTechnicianReadDto> CreateAsync(LabTechnicianCreateDto dto);
        Task<LabTechnicianReadDto> UpdateAsync(string ssn, LabTechnicianUpdateDto dto);
        Task<LabTechnicianReadDto> GetBySSNAsync(string ssn);
        Task<PaginatedResult<LabTechnicianReadDto>> GetAllAsync(PaginationParams pagination);
        Task DeleteAsync(string ssn);

    }
}
