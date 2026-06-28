using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IGenericRepo<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Delete(T entity);
        void Update(T entity);
    }
}

