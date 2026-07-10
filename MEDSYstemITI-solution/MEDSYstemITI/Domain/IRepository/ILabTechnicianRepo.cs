using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface ILabTechnicianRepo : IGenericRepository<LabTechnician>
    {
        // Entity-specific reads
        Task<LabTechnician?> GetByNameAsync(string name);
    }
}
