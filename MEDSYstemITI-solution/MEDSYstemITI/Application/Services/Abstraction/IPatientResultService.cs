using Application.DTOs.PatientResult;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface IPatientResultService
    {
        Task<PatientResultReadDto> CreateAsync(PatientResultCreateDto dto);
        Task<PatientResultReadDto> UpdateAsync(int id, PatientResultUpdateDto dto);
        Task<PatientResultReadDto> GetByIdAsync(int id);
        Task<PaginatedResult<PatientResultReadDto>> GetByPatientAsync(int patientId, PaginationParams pagination);
    }
}
