using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class RequestLabTestRepository : IRequestLabTestRepo
    {
        private readonly ApplicationDbContext _context;

        public RequestLabTestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RequestLabTest?> GetByIdsAsync(int requestLabId, int labTestId)
        {
            return await _context.RequestLabTests
                .AsNoTracking()
                .Where(rlt => rlt.RequestLabId == requestLabId && rlt.LabTestId == labTestId)
                .Include(rlt => rlt.LabTest)
                .Include(rlt => rlt.RequestLab)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<RequestLabTest>> GetPendingTestsAsync(int requestLabId)
        {
            return await _context.RequestLabTests
                .AsNoTracking()
                .Where(rlt => rlt.RequestLabId == requestLabId && rlt.Status == RequestLabTestStatus.Pending)
                .Include(rlt => rlt.LabTest)
                .ToListAsync();
        }

        public async Task<IEnumerable<RequestLabTest>> GetCompletedTestsAsync(int requestLabId)
        {
            return await _context.RequestLabTests
                .AsNoTracking()
                .Where(rlt => rlt.RequestLabId == requestLabId && rlt.Status == RequestLabTestStatus.Completed)
                .Include(rlt => rlt.LabTest)
                .ToListAsync();
        }

        public async Task<IEnumerable<RequestLabTest>> GetByRequestLabAsync(int requestLabId)
        {
            return await _context.RequestLabTests
                .AsNoTracking()
                .Where(rlt => rlt.RequestLabId == requestLabId)
                .Include(rlt => rlt.LabTest)
                .ToListAsync();
        }

        public async Task UpdateStatusAsync(int requestLabId, int labTestId, RequestLabTestStatus status)
        {
            var entity = await _context.RequestLabTests
                .Where(rlt => rlt.RequestLabId == requestLabId && rlt.LabTestId == labTestId)
                .FirstOrDefaultAsync();

            if (entity == null)
                return;

            entity.Status = status;
            entity.UpdatedAt = System.DateTime.UtcNow;

            _context.RequestLabTests.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
