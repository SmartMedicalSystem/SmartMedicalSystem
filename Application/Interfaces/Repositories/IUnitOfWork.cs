using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Department> Departments { get; }

        Task<int> CompleteAsync();
    }
}