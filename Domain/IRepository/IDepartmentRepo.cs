using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IDepartmentRepo : IGenericRepository<Department>
    {
        // Entity-specific reads
        Task<Department?> GetByNameAsync(string name);
        Task<Department?> GetWithDoctorsAsync(int id);
    }
}
