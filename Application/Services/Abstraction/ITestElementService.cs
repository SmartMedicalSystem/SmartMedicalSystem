using Application.DTOs.TestElement;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface ITestElementService
    {
        Task<TestElementReadDto> CreateAsync(TestElementCreateDto dto);
        Task<TestElementReadDto> UpdateAsync(int id, TestElementUpdateDto dto);
        Task<TestElementReadDto> GetByIdAsync(int id);
        Task<PaginatedResult<TestElementReadDto>> GetAllAsync(PaginationParams pagination);
        Task DeleteAsync(int id);
    }
}
