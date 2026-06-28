using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IPatientRepo Patients { get; }
        IRequestLabsRepository RequestLabs { get; }
        Task<int> SaveChangesAsync();
    }
}
