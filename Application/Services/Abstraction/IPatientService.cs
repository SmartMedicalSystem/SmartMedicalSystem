using Application.DTOs.AI;
using Application.DTOs.Patient;
using Application.DTOs.Patients;
using Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface IPatientService
    {
        Task<PatientReadDto> CreateAsync(PatientCreateDto dto);
        Task<PatientReadDto> UpdateAsync(string ssn, PatientUpdateDto dto);
        Task<PaginatedResult<PatientReadDto>> GetAllAsync(PatientFilterDto pagination);
        Task<PatientReadDto> GetByIdAsync(int id);
        Task<PatientReadDto> UpdateAsync(int id, PatientUpdateDto dto);

        Task DeleteAsync(string ssn);

        Task<PatientReadDto> GetBySSNAsync(string ssn);

        /// <summary>
        /// Gathers every one of the patient's PatientResult AI summaries/reports/suggestions into a
        /// single consolidated report, plus one AI-synthesized cross-result overview, for the
        /// doctor to read/view in one place. Any PatientResult that hasn't been AI-analyzed yet is
        /// analyzed on the fly so the report is always complete.
        /// </summary>
        Task<PatientFullAIReportDto> GetFullAIReportAsync(int patientId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the stored full patient AI report content if one exists in the RAG store.
        /// </summary>
        Task<StoredFullReportDto?> GetStoredFullAIReportAsync(int patientId);

        /// <summary>
        /// Replaces the stored full patient AI report in the RAG store with the provided content.
        /// </summary>
        Task UpdateStoredFullAIReportAsync(int patientId, string content, CancellationToken cancellationToken = default);
    }
}
