using Application.DTOs.Patient;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface IPatientService
    {
        Task<PatientReadDto> CreateAsync(PatientCreateDto dto);
        Task<PatientReadDto> UpdateAsync(int id, PatientUpdateDto dto);
        Task<PatientReadDto> GetByIdAsync(int id);
        Task<PaginatedResult<PatientReadDto>> GetAllAsync(PaginationParams pagination);
        Task DeleteAsync(int id);
    }
}
