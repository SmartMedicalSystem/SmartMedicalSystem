using Application.DTOs.Element;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface IElementService
    {
        Task<ElementReadDto> CreateAsync(ElementCreateDto dto);
        Task<ElementReadDto> UpdateAsync(int id, ElementUpdateDto dto);
        Task<ElementReadDto> GetByIdAsync(int id);
        Task<PaginatedResult<ElementReadDto>> GetAllAsync(PaginationParams pagination);
        Task DeleteAsync(int id);
    }
}
