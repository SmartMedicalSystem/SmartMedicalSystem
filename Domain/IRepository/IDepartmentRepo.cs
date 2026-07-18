using Domain.Entities;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IDepartmentRepo : IGenericRepository<Department>
    {
        // ============================================================
        // Entity-specific reads
        // ============================================================

        /// <summary>
        /// Retrieves a department by ID with all related doctors and head doctor.
        /// </summary>
        Task<Department?> GetDepartmentWithDoctorsAsync(int id);

        /// <summary>
        /// Retrieves all departments with their doctor collections and head doctor.
        /// </summary>
        Task<IEnumerable<Department>> GetAllWithDoctorsAsync();

        /// <summary>
        /// Retrieves only active departments with their doctor collections.
        /// </summary>
        Task<IEnumerable<Department>> GetActiveDepartmentsAsync();

        /// <summary>
        /// Checks if a department name is unique (case-insensitive).
        /// </summary>
        Task<bool> IsDepartmentNameUniqueAsync(string name, int? excludeId = null);

        // ============================================================
        // NEW: Pagination methods
        // ============================================================

        /// <summary>
        /// Retrieves paginated departments with doctors and head doctor.
        /// </summary>
        // في IDepartmentRepo
        Task<PaginatedResult<Department>> GetAllPaginatedAsync(
            PaginationParams pagination,
            string? searchTerm = null,
            string? statusFilter = null,
            string? creationDateFilter = null,
            string? managerFilter = null);

        // ============================================================
        // NEW: Search by name
        // ============================================================

        /// <summary>
        /// Retrieves a department by name (case-insensitive search).
        /// </summary>
        Task<Department?> GetByNameAsync(string name);
    }
}