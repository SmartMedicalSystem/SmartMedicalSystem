using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GenericRepo<TEntity> : IGenericRepo<TEntity>
        where TEntity : BaseEntity
    {
        protected readonly ApplicationDbContext context;
        protected readonly DbSet<TEntity> dbSet;

        public GenericRepo(ApplicationDbContext _context)
        {
            context = _context;
            dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            dbSet.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            dbSet.Remove(entity);
        }
    }
}