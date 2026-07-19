using Domain.Entities;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    /// <summary>
    /// Generic repository contract for entities that derive from BaseEntity
    /// (int Id, CreatedAt, UpdatedAt, IsDeleted).
    /// Holds the CRUD + pagination operations that were previously duplicated
    /// identically across every entity-specific repository interface.
    /// Entity-specific repositories (IDepartmentRepo, IDoctorRepo, ...) extend this
    /// interface and only declare the extra query methods that are unique to them.
    /// </summary>
    public interface IGenericRepository<T> where T : BaseEntity
    {
        // Create
        Task<T> AddAsync(T entity);

        // Read
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllActiveAsync();

        // Read with Pagination
        Task<PaginatedResult<T>> GetAllPaginatedAsync(PaginationParams pagination);
        Task<PaginatedResult<T>> GetAllActivePaginatedAsync(PaginationParams pagination);

        // Update
        Task<T> UpdateAsync(T entity);

        // Delete
        Task<bool> DeleteAsync(int id);
        Task<bool> SoftDeleteAsync(int id);

        // Check existence
        Task<bool> ExistsAsync(int id);
    }
}
