using Domain.Entities;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IPatientResultElementRepo : IGenericRepository<PatientResultElement>
    {
        // Entity-specific reads
        Task<IEnumerable<PatientResultElement>> GetByPatientResultAsync(int patientResultId);
        Task<IEnumerable<PatientResultElement>> GetByTestElementAsync(int testElementId);
        Task<IEnumerable<PatientResultElement>> GetByTechnicianAsync(int technicianId);

        // Entity-specific reads with Pagination
        Task<PaginatedResult<PatientResultElement>> GetByPatientResultPaginatedAsync(int patientResultId, PaginationParams pagination);
        Task<PaginatedResult<PatientResultElement>> GetByTestElementPaginatedAsync(int testElementId, PaginationParams pagination);
        Task<PaginatedResult<PatientResultElement>> GetByTechnicianPaginatedAsync(int technicianId, PaginationParams pagination);
    }
}
