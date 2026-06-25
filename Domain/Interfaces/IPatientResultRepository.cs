using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IPatientResultRepository
    {
        Task<IEnumerable<PatientResult>> GetAllAsync();

        Task<PatientResult?> GetByIdAsync(int id);

        Task AddAsync(PatientResult result);

        Task UpdateAsync(PatientResult result);

        Task DeleteAsync(PatientResult result);
    }
}
