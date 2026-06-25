using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class PatientResultRepository : IPatientResultRepository
    {
        private readonly AppDbContext _context;

        public PatientResultRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PatientResult>> GetAllAsync()
        {
            return await _context.PatientResults
                .Include(r => r.Patient)
                .Include(r => r.Test)
                .Include(r => r.ResultElements)
                    .ThenInclude(e => e.TestElement)
                .Include(r => r.ResultElements)
                    .ThenInclude(e => e.Technician)
                .ToListAsync();
        }

        public async Task<PatientResult?> GetByIdAsync(int id)
        {
            return await _context.PatientResults
                .Include(r => r.Patient)
                .Include(r => r.Test)
                .Include(r => r.ResultElements)
                    .ThenInclude(e => e.TestElement)
                .Include(r => r.ResultElements)
                    .ThenInclude(e => e.Technician)
                .FirstOrDefaultAsync(r => r.ResultId == id);
        }

        public async Task AddAsync(PatientResult result)
        {
            await _context.PatientResults.AddAsync(result);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PatientResult result)
        {
            _context.PatientResults.Update(result);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PatientResult result)
        {
            _context.PatientResults.Remove(result);
            await _context.SaveChangesAsync();
        }
    }
}
