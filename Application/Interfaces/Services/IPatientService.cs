using Application.DTOs.Patient;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDTO>> GetAllAsync();

        Task<PatientDTO?> GetByIdAsync(int id);
        Task<PatientDTO?> GetBySSNAsync(string ssn);
        Task CreateAsync(CreatePatientDTO dto);

        Task UpdateAsync(UpdatePatientDTO dto);

        Task DeleteAsync(int id);
    }
}
