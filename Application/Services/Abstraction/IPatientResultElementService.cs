using Application.DTOs.PatientResultElement;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface IPatientResultElementService
    {
        Task<PatientResultElementReadDto> CreateAsync(PatientResultElementCreateDto dto);
        Task<PatientResultElementReadDto> UpdateAsync(int id, PatientResultElementUpdateDto dto);
        Task<PaginatedResult<PatientResultElementReadDto>> GetByPatientResultAsync(int patientResultId, PaginationParams pagination);
    }
}
