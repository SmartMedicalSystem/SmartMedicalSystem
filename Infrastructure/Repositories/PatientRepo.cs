using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{

    public class PatientRepository
        : GenericRepo<Patient>, IPatientRepo
    {
        public PatientRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<Patient?> GetBySSNAsync(string ssn)
        {
            return await context.Patients
                .FirstOrDefaultAsync(p => p.SSN == ssn);
        }
    }
}
