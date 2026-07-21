using Domain.Entities;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface ILaboratoryRepo : IGenericRepository<Laboratory>
    {
        Task<Laboratory?> GetLaboratoryWithDetailsAsync(int id);
        Task<IEnumerable<Laboratory>> GetAllWithDetailsAsync();
        Task<IEnumerable<Laboratory>> GetActiveLaboratoriesAsync();
        Task<PaginatedResult<Laboratory>> GetAllPaginatedAsync(
            PaginationParams pagination,
            string? searchTerm = null,
            string? statusFilter = null);
        Task<bool> IsLaboratoryNameUniqueAsync(string name, int? excludeId = null);
        Task<IEnumerable<Laboratory>> GetForSelectAsync();

       
        Task<IEnumerable<Laboratory>> GetLaboratoriesBySpecialtyAsync(string specialty);
        Task<int> GetTotalTestsPerMonthAsync(int labId);
        Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);
       
    }
}