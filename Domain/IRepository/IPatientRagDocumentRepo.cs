using Domain.Entities;
using Microsoft.Data.SqlTypes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IPatientRagDocumentRepo : IGenericRepository<PatientRagDocument>
    {
        /// <summary>
        /// Returns the topK chunks belonging to <paramref name="patientId"/> that are closest to
        /// <paramref name="queryVector"/>, ordered nearest-first. Ranking is performed by SQL
        /// Server itself via VECTOR_DISTANCE (pushed down through EF.Functions.VectorDistance),
        /// not in application memory.
        /// </summary>
        Task<List<(PatientRagDocument Document, double Distance)>> SearchByPatientAsync(
            int patientId, SqlVector<float> queryVector, int topK, CancellationToken cancellationToken = default);

        /// <summary>
        /// Same as <see cref="SearchByPatientAsync"/> but across every active document (used for
        /// the cross-patient / GroupByPatient chatbot mode).
        /// </summary>
        Task<List<(PatientRagDocument Document, double Distance)>> SearchAllActiveAsync(
            SqlVector<float> queryVector, int topK, CancellationToken cancellationToken = default);

        /// <summary>All chunks previously indexed for a given PatientResult (so they can be replaced instead of duplicated on re-generation).</summary>
        Task<IEnumerable<PatientRagDocument>> GetByPatientResultAsync(int patientResultId);

        /// <summary>Hard-deletes every chunk indexed for a given PatientResult (used before re-indexing).</summary>
        Task RemoveByPatientResultAsync(int patientResultId);
    }
}
