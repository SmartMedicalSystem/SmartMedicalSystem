using Domain.Entities;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IPatientResultRepo : IGenericRepository<PatientResult>
    {
        // Entity-specific reads
        Task<IEnumerable<PatientResult>> GetByPatientAsync(int patientId);
        Task<IEnumerable<PatientResult>> GetBySessionAsync(int sessionId);
        Task<IEnumerable<PatientResult>> GetByLabTestAsync(int labTestId);
        Task<PatientResult?> GetWithResultElementsAsync(int id);

        // Entity-specific reads with Pagination
        Task<PaginatedResult<PatientResult>> GetByPatientPaginatedAsync(int patientId, PaginationParams pagination);
        Task<PaginatedResult<PatientResult>> GetBySessionPaginatedAsync(int sessionId, PaginationParams pagination);
        Task<PaginatedResult<PatientResult>> GetByLabTestPaginatedAsync(int labTestId, PaginationParams pagination);
    }
}
