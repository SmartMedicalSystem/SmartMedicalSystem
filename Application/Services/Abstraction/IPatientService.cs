using Application.DTOs.Patient;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface IPatientService
    {
        Task<PatientReadDto> CreateAsync(PatientCreateDto dto);
        Task<PatientReadDto> UpdateAsync(string ssn, PatientUpdateDto dto);
        Task<PaginatedResult<PatientReadDto>> GetAllAsync(PaginationParams pagination);
        Task DeleteAsync(string ssn);

        Task<PatientReadDto> GetBySSNAsync(string ssn);
    }
}
