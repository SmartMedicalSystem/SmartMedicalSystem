using Domain.Entities;
using Domain.Entities.Baseperson;
using Domain.IRepository;
using Infrastructure.Context;
using Infrastructure.Services;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    /// <summary>
    /// Unit of Work implementation. Lazily creates and caches one repository instance
    /// per aggregate, all sharing the same ApplicationDbContext, and exposes a
    /// SaveChangesAsync for operations that need to commit multiple repository
    /// changes together.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private bool _disposed;

        private IDepartmentRepo? _departments;
        private IDoctorRepo? _doctors;
        private ILabTechnicianRepo? _labTechnicians;
        private ILabTestRepo? _labTests;
        private ILabTestElementRepo? _labTestElements;
        private INotificationRepo? _notifications;
        private IPatientRepo? _patients;
        private IPatientResultRepo? _patientResults;
        private IPatientResultElementRepo? _patientResultElements;
        private IRequestLabsRepo? _requestLabs;
        private ISessionRepo? _sessions;
        private ITestElementRepo? _testElements;
        private IPersonGenericRepo? _personGeneric;
        private readonly IDataProtectionProvider _dataProtectionProvider;


        private readonly Dictionary<Type, object> _genericRepositories = new();

        public UnitOfWork(ApplicationDbContext context, IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
            _dataProtectionProvider = dataProtectionProvider;
        }

        public IDepartmentRepo Departments =>
            _departments ??= new DepartmentRepository(_context);

        public IDoctorRepo Doctors =>
            _doctors ??= new DoctorRepository(_context);

        public ILabTechnicianRepo LabTechnicians =>
            _labTechnicians ??= new LabTechnicianRepository(_context);

        public ILabTestRepo LabTests =>
            _labTests ??= new LabTestRepository(_context);

        public ILabTestElementRepo LabTestElements =>
            _labTestElements ??= new LabTestElementRepository(_context);

        public INotificationRepo Notifications =>
            _notifications ??= new NotificationRepository(_context);

        public IPatientRepo Patients =>
            _patients ??= new PatientRepository(_context);

        public IPatientResultRepo PatientResults =>
            _patientResults ??= new PatientResultRepository(_context);

        public IPatientResultElementRepo PatientResultElements =>
            _patientResultElements ??= new PatientResultElementRepository(_context);

        public IRequestLabsRepo RequestLabs =>
            _requestLabs ??= new RequestLabsRepository(_context);

        public ISessionRepo Sessions =>
            _sessions ??= new SessionRepository(_context);

        public ITestElementRepo TestElements =>
            _testElements ??= new TestElementRepository(_context);

        public IPersonGenericRepo PersonGeneric =>
            _personGeneric ??= new PersonGenericRepo<BasePerson>(_context, new EncryptionService(_dataProtectionProvider));
            

        /// <summary>
        /// Generic accessor for entities that don't have a dedicated specialized repository.
        /// Instances are cached per entity type for the lifetime of this Unit of Work.
        /// </summary>
        public IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            var type = typeof(T);

            if (!_genericRepositories.TryGetValue(type, out var repository))
            {
                repository = new GenericRepository<T>(_context);
                _genericRepositories[type] = repository;
            }

            return (IGenericRepository<T>)repository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }

            _disposed = true;
        }
    }
}
