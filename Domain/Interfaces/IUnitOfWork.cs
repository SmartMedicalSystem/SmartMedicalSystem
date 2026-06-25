using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<TestElements> TestElements { get; }

        Task<int> SaveChangesAsync();
    }
}
