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

        /// <summary>
        /// Backs the dashboard grid: combines Status/Priority/Test/Doctor filters
        /// with a free-text search over patient name, SSN and doctor name.
        /// </summary>
        Task<PaginatedResult<RequestLabs>> GetFilteredAsync(RequestLabsFilterParams filter, PaginationParams pagination);

        /// <summary>Backs the 3 stat cards on top of the dashboard.</summary>
        Task<RequestLabsStats> GetStatsAsync();
    }
}