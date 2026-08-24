using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Context;
using Microsoft.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    /// <summary>
    /// Repository for the RAG vector-store table. Similarity search is pushed all the way down to
    /// SQL Server 2025's native VECTOR type: EF.Functions.VectorDistance("cosine", ...) is
    /// translated to the T-SQL VECTOR_DISTANCE function, so SQL Server computes and orders by
    /// distance itself instead of the candidate rows being pulled into memory and scored in C#
    /// (see the old Application.Services.AI.RagService for the previous brute-force approach).
    /// </summary>
    public class PatientRagDocumentRepository : GenericRepository<PatientRagDocument>, IPatientRagDocumentRepo
    {
        public PatientRagDocumentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<(PatientRagDocument Document, double Distance)>> SearchByPatientAsync(
            int patientId, SqlVector<float> queryVector, int topK, CancellationToken cancellationToken = default)
        {
            var rows = await _context.PatientRagDocuments
                .Where(d => d.PatientId == patientId && !d.IsDeleted)
                .Select(d => new
                {
                    Document = d,
                    Distance = EF.Functions.VectorDistance("cosine", d.EmbeddingVector, queryVector)
                })
                .OrderBy(x => x.Distance)
                .Take(topK)
                .ToListAsync(cancellationToken);

            return rows.Select(x => (x.Document, x.Distance)).ToList();
        }

        public async Task<List<(PatientRagDocument Document, double Distance)>> SearchAllActiveAsync(
            SqlVector<float> queryVector, int topK, CancellationToken cancellationToken = default)
        {
            var rows = await _context.PatientRagDocuments
                .Where(d => !d.IsDeleted)
                .Select(d => new
                {
                    Document = d,
                    Distance = EF.Functions.VectorDistance("cosine", d.EmbeddingVector, queryVector)
                })
                .OrderBy(x => x.Distance)
                .Take(topK)
                .ToListAsync(cancellationToken);

            return rows.Select(x => (x.Document, x.Distance)).ToList();
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

        public async Task RemoveByPatientAndSourceAsync(int patientId, Domain.Enums.RagSourceType sourceType)
        {
            var existing = await _context.PatientRagDocuments
                .Where(d => d.PatientId == patientId && d.PatientResultId == null && d.SourceType == sourceType)
                .ToListAsync();

            if (existing.Count == 0)
                return;

            _context.PatientRagDocuments.RemoveRange(existing);
            await _context.SaveChangesAsync();
        }

        public async Task<PatientRagDocument?> GetByPatientAndSourceAsync(int patientId, Domain.Enums.RagSourceType sourceType)
        {
            return await _context.PatientRagDocuments
                .Where(d => d.PatientId == patientId && d.PatientResultId == null && d.SourceType == sourceType && !d.IsDeleted)
                .FirstOrDefaultAsync();
        }
    }
}
