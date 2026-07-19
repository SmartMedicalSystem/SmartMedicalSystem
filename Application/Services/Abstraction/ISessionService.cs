using Application.DTOs.Session;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface ISessionService
    {
        Task<SessionReadDto> CreateAsync(SessionCreateDto dto);
        Task<SessionReadDto> UpdateAsync(int id, SessionUpdateDto dto);
        Task<SessionReadDto> GetByIdAsync(int id);
        Task<PaginatedResult<SessionReadDto>> GetByPatientAsync(int patientId, PaginationParams pagination);
        Task DeleteAsync(int id);
    }
}
