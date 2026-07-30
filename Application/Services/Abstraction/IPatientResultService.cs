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
        Task<PatientResultReadDto> UpdateAsync(int id, PatientResultUpdateDto dto);
        Task<PatientResultReadDto> GetByIdAsync(int id);
        Task<PaginatedResult<PatientResultReadDto>> GetByPatientAsync(int patientId, PaginationParams pagination);

        /// <summary>
        /// Uses AI to summarize every PatientResultElement belonging to this PatientResult and
        /// generate its classified report + suggestions, persists them, and indexes the generated
        /// text into the RAG vector store. Same pipeline the MCP tools call.
        /// </summary>
        Task<PatientResultAIAnalysisDto> GenerateAIAnalysisAsync(int id, CancellationToken cancellationToken = default);
    }
}
