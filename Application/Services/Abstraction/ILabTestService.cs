using Application.DTOs.LabTest;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface ILabTestService
    {
        Task<LabTestReadDto> CreateAsync(LabTestCreateDto dto);
        Task<LabTestReadDto> UpdateAsync(int id, LabTestUpdateDto dto);
        Task<LabTestReadDto> GetByIdAsync(int id);
        Task<PaginatedResult<LabTestReadDto>> GetAllAsync(PaginationParams pagination);
        Task DeleteAsync(int id);
    }
}
