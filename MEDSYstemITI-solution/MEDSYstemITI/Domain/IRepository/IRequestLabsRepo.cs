using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IRequestLabsRepo : IGenericRepository<RequestLabs>
    {
        // Entity-specific reads
        Task<IEnumerable<RequestLabs>> GetBySessionAsync(int sessionId);
        Task<IEnumerable<RequestLabs>> GetByStatusAsync(LabRequestStatus status);
        Task<RequestLabs?> GetWithLabTestsAsync(int id);

        // Entity-specific reads with Pagination
        Task<PaginatedResult<RequestLabs>> GetBySessionPaginatedAsync(int sessionId, PaginationParams pagination);
        Task<PaginatedResult<RequestLabs>> GetByStatusPaginatedAsync(LabRequestStatus status, PaginationParams pagination);
    }
}
