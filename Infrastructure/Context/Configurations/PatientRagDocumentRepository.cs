using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    /// <summary>
    /// Repository for the RAG vector-store table. Similarity search itself lives in
    /// Application.Services.RagService (it needs the embedding client + cosine-similarity math,
    /// which belong in the Application layer); this repository just serves the candidate rows.
    /// </summary>
    public class PatientRagDocumentRepository : GenericRepository<PatientRagDocument>, IPatientRagDocumentRepo
    {
        public PatientRagDocumentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PatientRagDocument>> GetByPatientAsync(int patientId)
        {
            return await _context.PatientRagDocuments
                .Where(d => d.PatientId == patientId && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PatientRagDocument>> GetAllActiveDocumentsAsync()
        {
            return await _context.PatientRagDocuments
                .Where(d => !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PatientRagDocument>> GetByPatientResultAsync(int patientResultId)
        {
            return await _context.PatientRagDocuments
                .Where(d => d.PatientResultId == patientResultId && !d.IsDeleted)
                .ToListAsync();
        }

        public async Task RemoveByPatientResultAsync(int patientResultId)
        {
            var existing = await _context.PatientRagDocuments
                .Where(d => d.PatientResultId == patientResultId)
                .ToListAsync();

            if (existing.Count == 0)
                return;

            _context.PatientRagDocuments.RemoveRange(existing);
            await _context.SaveChangesAsync();
        }
    }
}
