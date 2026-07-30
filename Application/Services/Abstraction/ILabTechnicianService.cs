using Application.DTOs.LabTechnician;
using Domain.Models;

namespace Application.Services.Abstraction
{
    public interface ILabTechnicianService
    {
        Task<LabTechnicianReadDto> CreateAsync(LabTechnicianCreateDto dto);
        Task<LabTechnicianReadDto> UpdateAsync(string ssn, LabTechnicianUpdateDto dto);
        Task<LabTechnicianReadDto> GetBySSNAsync(string ssn);
        Task<PaginatedResult<LabTechnicianReadDto>> GetAllAsync(LabTechnicianFilterDto filter);
        Task DeleteAsync(string ssn);

        // ===== NEW: Get technicians by Laboratory =====
        Task<PaginatedResult<LabTechnicianReadDto>> GetByLaboratoryIdAsync(
            int laboratoryId,
            PaginationParams pagination,
            string? searchTerm = null);

        Task<PaginatedResult<LabTechnicianReadDto>>
    GetAvailableForLaboratoryAsync(
        int laboratoryId,
        PaginationParams pagination,
        string? searchTerm = null);
    }

}

