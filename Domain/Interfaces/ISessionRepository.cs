using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface ISessionRepository
    {
        Task<IEnumerable<Session>> GetAllAsync();

        Task<Session?> GetByIdAsync(int id);

        Task AddAsync(Session session);

        Task UpdateAsync(Session session);

        Task DeleteAsync(Session session);
    }
}
