using Infrastructure.Context;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IGenericRepository<TestElements> TestElements { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            TestElements = new GenericRepository<TestElements>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}

