using Application.DTOs.PatientResultDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.PatientResultServiceAbstract
{
    public interface IPatientResultService
    {
        Task<IEnumerable<PatientResultDto>> GetAllAsync();

        Task<PatientResultDto?> GetByIdAsync(int id);

        Task CreateAsync(CreatePatientResultDto dto);

        Task DeleteAsync(int id);
    }
}
