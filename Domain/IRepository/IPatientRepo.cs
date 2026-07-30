using Domain.Entities;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IPatientRepo : IGenericRepository<Patient>
    {
        // Entity-specific reads
        Task<IEnumerable<Patient>> GetAllWithSessionsAsync();

        // Entity-specific reads with Pagination
        Task<PaginatedResult<Patient>> GetAllWithSessionsPaginatedAsync(PaginationParams pagination);
    }
}
