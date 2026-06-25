using Application.DTOs.SessionDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.SessionServiceAbstract
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionDto>> GetAllAsync();

        Task<SessionDto?> GetByIdAsync(int id);

        Task CreateAsync(CreateSessionDto dto);

        Task UpdateAsync(UpdateSessionDto dto);

        Task DeleteAsync(int id);
    }
}
