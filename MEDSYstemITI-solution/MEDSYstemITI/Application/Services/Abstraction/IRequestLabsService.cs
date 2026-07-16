using Application.DTOs.RequestLabs;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface IRequestLabsService
    {
        Task<RequestLabsReadDto> CreateAsync(RequestLabsCreateDto dto);
        Task<RequestLabsReadDto> UpdateStatusAsync(int id, RequestLabsUpdateStatusDto dto);
        Task<RequestLabsReadDto> GetByIdAsync(int id);
        Task<PaginatedResult<RequestLabsReadDto>> GetBySessionAsync(int sessionId, PaginationParams pagination);

        /// <summary>Backs the dashboard grid (filters + search + pagination).</summary>
        Task<PaginatedResult<RequestLabsReadDto>> GetFilteredAsync(RequestLabsFilterDto filter, PaginationParams pagination);

        /// <summary>Backs the 3 stat cards on top of the dashboard.</summary>
        Task<RequestLabsStatsDto> GetStatsAsync();
    }
}