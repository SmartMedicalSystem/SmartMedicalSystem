using Domain.Interfaces;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;
        public IDoctorRepo Doctors { get; }
        public UnitOfWork(ApplicationDbContext _context,IDoctorRepo doctorRepo)
        {
            this.Doctors = doctorRepo;
            this.context = _context;
        }

       

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }
    }
}
