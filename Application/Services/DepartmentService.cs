using Application.DTOs.Department;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments =
                await _unitOfWork.Departments.GetAllAsync();

            return departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name
            });
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            var department =
                await _unitOfWork.Departments.GetByIdAsync(id);

            if (department == null)
                return null;

            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name
            };
        }

        public async Task CreateAsync(CreateDepartmentDto dto)
        {
            var department = new Department
            {
                Name = dto.Name
            };

            await _unitOfWork.Departments.AddAsync(department);

            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(int id, UpdateDepartmentDto dto)
        {
            var department =
                await _unitOfWork.Departments.GetByIdAsync(id);

            if (department == null)
                throw new Exception("Department not found");

            department.Name = dto.Name;

            _unitOfWork.Departments.Update(department);

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var department =
                await _unitOfWork.Departments.GetByIdAsync(id);

            if (department == null)
                throw new Exception("Department not found");

            _unitOfWork.Departments.Delete(department);

            await _unitOfWork.CompleteAsync();
        }
    }
}