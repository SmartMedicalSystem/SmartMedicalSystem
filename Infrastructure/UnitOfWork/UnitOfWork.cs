using Domain.Interfaces;
using Infrastructure.Context;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IPatientRepo Patients { get; }
        public IRequestLabsRepository RequestLabs { get; }
        public UnitOfWork(
            ApplicationDbContext context,
            IPatientRepo patientRepository,
            IRequestLabsRepository requestLabs)
        {
            _context = context;
            Patients = patientRepository;
            RequestLabs = requestLabs;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
