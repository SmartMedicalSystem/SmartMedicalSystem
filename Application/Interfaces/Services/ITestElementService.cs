using Application.DTOs.TestElementsDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface ITestElementService
    {
        Task<TestElementDto> GetByIdAsync(int testElementId);
        Task<IEnumerable<TestElementDto>> GetAllAsync();
        Task<IEnumerable<TestElementDto>> GetByTestIdAsync(int testId);
        Task<TestElementDto> CreateAsync(CreateTestElementDto dto);
        Task<bool> UpdateAsync(int testElementId, UpdateTestElementDto dto);
        Task<bool> DeleteAsync(int testElementId);
    }
}
