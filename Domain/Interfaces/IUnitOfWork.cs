using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IDoctorRepo Doctors { get; }

        Task<int> SaveChangesAsync();
    }
}
