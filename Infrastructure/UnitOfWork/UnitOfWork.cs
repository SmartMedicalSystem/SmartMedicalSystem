using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public ILabTestRepository LabTests { get; }

        public UnitOfWork(
            AppDbContext context,
            ILabTestRepository labTestRepository)
        {
            _context = context;

            LabTests = labTestRepository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
