using Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class LabTestService : ILabTestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LabTestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LabTestDto>> GetAllAsync()
        {
            var tests = await _unitOfWork.LabTests.GetAllAsync();

            return tests.Select(x => new LabTestDto
            {
                Id = x.Id,
                TestName = x.TestName,
                Description = x.Description
            });
        }

        public async Task<LabTestDto?> GetByIdAsync(int id)
        {
            var test = await _unitOfWork.LabTests.GetByIdAsync(id);

            if (test == null)
                return null;

            return new LabTestDto
            {
                Id = test.Id,
                TestName = test.TestName,
                Description = test.Description
            };
        }

        public async Task CreateAsync(CreateLabTestDto dto)
        {
            var test = new LabTest(
                dto.TestName,
                dto.Description
            );

            await _unitOfWork.LabTests.AddAsync(test);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateLabTestDto dto)
        {
            var test = await _unitOfWork.LabTests.GetByIdAsync(id);

            if (test == null)
                return false;

            test.Update(
                dto.TestName,
                dto.Description
            );

            _unitOfWork.LabTests.Update(test);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var test = await _unitOfWork.LabTests.GetByIdAsync(id);

            if (test == null)
                return false;

            _unitOfWork.LabTests.Delete(test);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
