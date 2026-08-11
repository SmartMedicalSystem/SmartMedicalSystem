using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface ILabTestRepo : IGenericRepository<LabTest>
    {
        // Entity-specific reads
        Task<LabTest?> GetByNameAsync(string testName);
        Task<LabTest?> GetWithElementsAsync(int id);

        //Task<IEnumerable<LabTest>> GetByStatusAsync(LabTestStatus status);
        //Task<PaginatedResult<LabTest>> GetByStatusPaginatedAsync(LabTestStatus status, PaginationParams pagination);

        // ===== NEW: Get Lab Tests by Laboratory =====
        Task<PaginatedResult<LabTest>> GetByLaboratoryIdAsync(
            int laboratoryId,
            PaginationParams pagination,
            string? searchTerm = null);

    }
}
