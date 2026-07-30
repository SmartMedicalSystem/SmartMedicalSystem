using Application.DTOs.RequestLabsDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IRequestLabsService
    {

    Task<RequestLabsDto> CreateAsync (CreateRequestLabsDto dto);

        Task<RequestLabsDto> GetByIdAsync (int id);

        Task<IEnumerable<RequestLabsDto>> GetAllAsync();
        Task UpdateStatusAsync(int id, UpdateRequestLabsStatusDto dto);
        Task DeleteAsync (int id);

       
    }
}
