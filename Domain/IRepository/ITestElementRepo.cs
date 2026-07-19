using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface ITestElementRepo : IGenericRepository<TestElement>
    {
        // Entity-specific reads
        Task<TestElement?> GetByNameAsync(string elementName);
    }
}
