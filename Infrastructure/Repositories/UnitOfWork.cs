using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IGenericRepository<Department> Departments { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Departments =
                new GenericRepository<Department>(context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}