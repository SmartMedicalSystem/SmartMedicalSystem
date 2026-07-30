using Application.DTOs.Laboratory;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Abstraction
{
    public interface ILaboratoryService
    {
        Task<LaboratoryReadDto> CreateAsync(LaboratoryCreateDto dto);
        Task<LaboratoryReadDto> UpdateAsync(int id, LaboratoryUpdateDto dto);
        Task<LaboratoryReadDto> GetByIdAsync(int id);

        Task<IEnumerable<LaboratoryReadDto>> GetActiveLaboratoriesAsync();
        Task<IEnumerable<LaboratoryForSelectDto>> GetForSelectAsync();
        Task<bool> IsLaboratoryNameUniqueAsync(string name, int? excludeId = null);
        Task DeleteAsync(int id);
        Task<PaginatedResult<LaboratoryReadDto>> GetAllAsync(
    PaginationParams pagination,
    string? searchTerm = null,
    string? statusFilter = null);

      
    }
}