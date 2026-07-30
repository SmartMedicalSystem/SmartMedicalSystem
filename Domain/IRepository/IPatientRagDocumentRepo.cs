using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IPatientRagDocumentRepo : IGenericRepository<PatientRagDocument>
    {
        /// <summary>All active (non-deleted) chunks belonging to a patient - the candidate set for similarity search.</summary>
        Task<IEnumerable<PatientRagDocument>> GetByPatientAsync(int patientId);

        /// <summary>All active chunks across every patient - used when the chatbot query isn't scoped to one patient.</summary>
        Task<IEnumerable<PatientRagDocument>> GetAllActiveDocumentsAsync();

        /// <summary>All chunks previously indexed for a given PatientResult (so they can be replaced instead of duplicated on re-generation).</summary>
        Task<IEnumerable<PatientRagDocument>> GetByPatientResultAsync(int patientResultId);

        /// <summary>Hard-deletes every chunk indexed for a given PatientResult (used before re-indexing).</summary>
        Task RemoveByPatientResultAsync(int patientResultId);
    }
}
