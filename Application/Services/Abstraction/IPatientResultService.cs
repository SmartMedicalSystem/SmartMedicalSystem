using Application.DTOs.AI;
using Application.DTOs.PatientResult;
using Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface IPatientResultService
    {
        Task<PatientResultReadDto> CreateAsync(PatientResultCreateDto dto);
        Task<PatientResultReadDto> UpdateAsync(int id, PatientResultUpdateDto dto, CancellationToken cancellationToken);
        Task<PatientResultReadDto> UpdateStatusAsync(int id, Domain.Enums.PatinetResultAIReportStatus status);
        Task<PatientResultReadDto> GetByIdAsync(int id);
        Task<PaginatedResult<PatientResultReadDto>> GetByPatientAsync(int patientId, PaginationParams pagination);
        Task<PaginatedResult<PatientResultReadDto>> GetByDoctorAsync(int doctorId, PaginationParams pagination);


        //Task<PatientResultAIAnalysisDto> UpdateAIAnalysisAsync(int id, PatientResultAIAnalysisDto updatedAnalysis, CancellationToken cancellationToken = default);
        /// <summary>
        /// Uses AI to summarize every PatientResultElement belonging to this PatientResult and
        /// generate its classified report + suggestions, persists them, and indexes the generated
        /// text into the RAG vector store. Same pipeline the MCP tools call.
        /// </summary>
        Task<PatientResultAIAnalysisDto> GenerateAIAnalysisAsync(int id, CancellationToken cancellationToken = default);
    }
}
