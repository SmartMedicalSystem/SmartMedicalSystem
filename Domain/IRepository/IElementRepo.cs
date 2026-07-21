using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IElementRepo : IGenericRepository<Element>
    {
        Task<Element?> GetByNameAsync(string name);
    }
}
