using Application.DTOs.AI;
using Application.DTOs.Patient;
using Domain.Models;
using System.Threading;
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

        /// <summary>
        /// Gathers every one of the patient's PatientResult AI summaries/reports/suggestions into a
        /// single consolidated report, plus one AI-synthesized cross-result overview, for the
        /// doctor to read/view in one place. Any PatientResult that hasn't been AI-analyzed yet is
        /// analyzed on the fly so the report is always complete.
        /// </summary>
        Task<PatientFullAIReportDto> GetFullAIReportAsync(int patientId, CancellationToken cancellationToken = default);
    }
}
