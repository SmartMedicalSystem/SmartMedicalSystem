using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface ILabTestRepo : IGenericRepository<LabTest>
    {
        // Entity-specific reads
        Task<LabTest?> GetByNameAsync(string testName);
        Task<LabTest?> GetWithElementsAsync(int id);
    }
}
