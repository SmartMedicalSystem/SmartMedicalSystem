using Domain.Entities;
using Domain.IRepository;
using Domain.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    /// <summary>
    /// Generic repository implementation for entities deriving from BaseEntity.
    /// Contains the exact same logic that was previously duplicated in every
    /// entity-specific repository (AddAsync, UpdateAsync, DeleteAsync, SoftDeleteAsync,
    /// ExistsAsync, and the "plain" versions of GetByIdAsync / GetAllAsync / GetAllPaginatedAsync).
    ///
    /// Entity-specific repositories inherit from this class and only override
    /// GetByIdAsync / GetAllAsync / GetAllPaginatedAsync when they need extra
    /// .Include(...) / .OrderBy(...) behavior. All other members (Add/Update/Delete/
    /// SoftDelete/Exists/GetAllActive*) are reused as-is, so the exact original
    /// behavior is preserved everywhere.
    /// </summary>
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        public virtual async Task<T> AddAsync(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsDeleted = false;

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// Retrieves an entity by ID, excluding soft-deleted records.
        /// Entities that need eager-loaded navigation properties override this method.
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Where(e => e.Id == id && !e.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all active (non soft-deleted) entities.
        /// Entities that need eager-loaded navigation properties / custom ordering override this method.
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .ToListAsync();
        }

        /// <summary>
        /// Alias for GetAllAsync - returns all active entities.
        /// Calls the (possibly overridden) GetAllAsync, so derived classes automatically
        /// get the correct behavior without needing to override this member too.
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllActiveAsync()
        {
            return await GetAllAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of all active entities.
        /// Entities that need eager-loaded navigation properties / custom ordering override this method.
        /// </summary>
        public virtual async Task<PaginatedResult<T>> GetAllPaginatedAsync(PaginationParams pagination)
        {
            var totalCount = await _dbSet
                .Where(e => !e.IsDeleted)
                .CountAsync();

            var items = await _dbSet
                .Where(e => !e.IsDeleted)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<T>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Alias for GetAllPaginatedAsync - returns all active entities paginated.
        /// </summary>
        public virtual async Task<PaginatedResult<T>> GetAllActivePaginatedAsync(PaginationParams pagination)
        {
            return await GetAllPaginatedAsync(pagination);
        }

        /// <summary>
        /// Updates an existing entity record.
        /// </summary>
        public virtual async Task<T> UpdateAsync(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;

            _dbSet.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// Permanently deletes an entity from the database.
        /// </summary>
        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Soft deletes an entity by marking IsDeleted = true.
        /// </summary>
        public virtual async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                return false;

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            _dbSet.Update(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Checks if an entity exists (active records only).
        /// </summary>
        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet
                .AnyAsync(e => e.Id == id && !e.IsDeleted);
        }
    }
}
