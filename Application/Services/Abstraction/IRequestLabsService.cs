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
        Task<PaginatedResult<RequestLabsReadDto>> QueryAsync(PaginationParams pagination, string? search = null, Domain.Enums.LabRequestStatus? status = null, Domain.Enums.LabRequestPriority? priority = null, int? labTestId = null, int? doctorId = null);

        Task<RequestLabsStatisticsDto> GetStatisticsAsync();
    }
}
