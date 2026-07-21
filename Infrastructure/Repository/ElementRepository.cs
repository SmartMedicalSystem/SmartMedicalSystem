using Domain.Entities;
using Domain.IRepository;
using Domain.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class ElementRepository : GenericRepository<Element>, IElementRepo
    {
        public ElementRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<Element?> GetByIdAsync(int id)
        {
            return await _context.Elements
                .Where(e => e.Id == id && !e.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public override async Task<IEnumerable<Element>> GetAllAsync()
        {
            return await _context.Elements
                .Where(e => !e.IsDeleted)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<Element?> GetByNameAsync(string name)
        {
            return await _context.Elements
                .Where(e => !e.IsDeleted && e.Name.ToLower().Contains(name.ToLower()))
                .FirstOrDefaultAsync();
        }

        public override async Task<PaginatedResult<Element>> GetAllPaginatedAsync(PaginationParams pagination)
        {
            var totalCount = await _context.Elements
                .Where(e => !e.IsDeleted)
                .CountAsync();

            var items = await _context.Elements
                .Where(e => !e.IsDeleted)
                .OrderBy(e => e.Name)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Element>.Create(items, totalCount, pagination);
        }
    }
}
