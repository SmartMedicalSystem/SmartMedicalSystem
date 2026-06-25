using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    internal class IUnitOfWork
    {
        ILabTestRepository LabTests { get; }

        Task<int> SaveChangesAsync();
    }
}
