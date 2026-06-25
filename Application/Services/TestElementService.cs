using Application.DTOs.TestElementsDto;
using Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    internal class TestElementService
    {
        public class TestElementService : ITestElementService
        {
            private readonly IUnitOfWork _unitOfWork;

            public TestElementService(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<TestElementDto> GetByIdAsync(int testElementId)
            {
                var entity = await _unitOfWork.TestElements.GetByIdAsync(testElementId);
                if (entity == null) return null;

                return MapToDto(entity);
            }

            public async Task<IEnumerable<TestElementDto>> GetAllAsync()
            {
                var entities = await _unitOfWork.TestElements.GetAllAsync();
                return entities.Select(MapToDto);
            }

            public async Task<IEnumerable<TestElementDto>> GetByTestIdAsync(int testId)
            {
                var entities = await _unitOfWork.TestElements.FindAsync(te => te.TestId == testId);
                return entities.Select(MapToDto);
            }

            public async Task<TestElementDto> CreateAsync(CreateTestElementDto dto)
            {
                var entity = new TestElements(dto.TestId, dto.ElementName, dto.Unit, dto.NormalMin, dto.NormalMax);

                await _unitOfWork.TestElements.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                return MapToDto(entity);
            }

            public async Task<bool> UpdateAsync(int testElementId, UpdateTestElementDto dto)
            {
                var entity = await _unitOfWork.TestElements.GetByIdAsync(testElementId);
                if (entity == null) return false;

                entity.UpdateDetails(dto.ElementName, dto.Unit, dto.NormalMin, dto.NormalMax);

                _unitOfWork.TestElements.Update(entity);
                var affectedRows = await _unitOfWork.SaveChangesAsync();
                return affectedRows > 0;
            }

            public async Task<bool> DeleteAsync(int testElementId)
            {
                var entity = await _unitOfWork.TestElements.GetByIdAsync(testElementId);
                if (entity == null) return false;

                _unitOfWork.TestElements.Delete(entity);
                var affectedRows = await _unitOfWork.SaveChangesAsync();
                return affectedRows > 0;
            }

            private static TestElementDto MapToDto(TestElements entity)
            {
                return new TestElementDto
                {
                    TestElementId = entity.TestElementId,
                    TestId = entity.TestId,
                    TestName = entity.LabTest?.TestName,
                    ElementName = entity.ElementName,
                    Unit = entity.Unit,
                    NormalMin = entity.NormalMin,
                    NormalMax = entity.NormalMax
                };
            }
        }
    }
}
