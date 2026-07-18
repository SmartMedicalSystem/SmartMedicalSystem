using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    /// <summary>
    /// Unit of Work contract. Exposes one repository property per aggregate/entity
    /// (backed by the same ApplicationDbContext instance) plus a generic repository
    /// accessor and a shared SaveChangesAsync for scenarios that need to commit
    /// changes across more than one repository in a single transaction.
    ///
    /// NOTE: the individual repositories still call SaveChangesAsync internally for
    /// each of their own operations (exactly as before this refactor), so existing
    /// call sites keep behaving exactly the same. UnitOfWork.SaveChangesAsync() is
    /// provided in addition, for new code that wants to batch multiple repository
    /// operations into a single DB round trip/transaction.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IDepartmentRepo Departments { get; }
        IDoctorRepo Doctors { get; }
        ILabTechnicianRepo LabTechnicians { get; }
        ILabTestRepo LabTests { get; }
        ILabTestElementRepo LabTestElements { get; }
        INotificationRepo Notifications { get; }
        IPatientRepo Patients { get; }
        IPatientResultRepo PatientResults { get; }
        IPatientResultElementRepo PatientResultElements { get; }
        IRequestLabsRepo RequestLabs { get; }
        ISessionRepo Sessions { get; }
        ITestElementRepo TestElements { get; }

        IPersonGenericRepo PersonGeneric { get; }

        
        /// <summary>
        /// Generic accessor for entities that don't have a dedicated specialized
        /// repository. Returns a shared GenericRepository&lt;T&gt; instance bound to
        /// the same DbContext as every other repository exposed by this Unit of Work.
        /// </summary>
        IGenericRepository<T> Repository<T>() where T : BaseEntity;

        /// <summary>
        /// Persists all pending changes tracked by the underlying DbContext.
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
