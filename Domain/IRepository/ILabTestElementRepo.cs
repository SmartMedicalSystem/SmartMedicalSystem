using Domain.Entities;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface ILabTestElementRepo
    {
        // Create
        Task<LabTestElement> AddAsync(LabTestElement labTestElement);

        // Read
        Task<LabTestElement?> GetByIdAsync(int labTestId, int testElementId);
        Task<IEnumerable<LabTestElement>> GetByLabTestAsync(int labTestId);
        Task<IEnumerable<LabTestElement>> GetByTestElementAsync(int testElementId);
        Task<IEnumerable<LabTestElement>> GetAllAsync();

        // Read with Pagination
        Task<PaginatedResult<LabTestElement>> GetAllPaginatedAsync(PaginationParams pagination);
        Task<PaginatedResult<LabTestElement>> GetByLabTestPaginatedAsync(int labTestId, PaginationParams pagination);
        Task<PaginatedResult<LabTestElement>> GetByTestElementPaginatedAsync(int testElementId, PaginationParams pagination);

        // Update
        Task<LabTestElement> UpdateAsync(LabTestElement labTestElement);

        // Delete
        Task<bool> DeleteAsync(int labTestId, int testElementId);

        // Check existence
        Task<bool> ExistsAsync(int labTestId, int testElementId);
    }
}
