using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IUnitOfWork
    {
        ILabTestRepository LabTests { get; }

        Task<int> SaveChangesAsync();
    }
}
