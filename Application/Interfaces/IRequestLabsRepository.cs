using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IRequestLabsRepository
    {
        Task<RequestLabs> GetByIdAsync (int id);

        Task<IEnumerable<RequestLabs>> GetAllAsync();

        Task AddAsync(RequestLabs requestLabs);
        Task UpdateAsync (RequestLabs requestLabs);

        Task DeleteAsync (RequestLabs requestLabs);

        Task<IEnumerable<RequestLabs>>
        GetBySessionIdAsync(int sessionId);

    }
}
