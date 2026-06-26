using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface ILabTestService
    {
        Task<IEnumerable<LabTestDto>> GetAllAsync();

        Task<LabTestDto?> GetByIdAsync(int id);

        Task CreateAsync(CreateLabTestDto dto);

        Task<bool> UpdateAsync(int id, UpdateLabTestDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
