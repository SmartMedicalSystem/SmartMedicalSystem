using Application.Common;
using Application.DTOs.LabTechnician;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.Entities;
using Domain.IRepository;
using Domain.Models;

namespace Application.Services
{
    public class LabTechnicianService : ILabTechnicianService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public LabTechnicianService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<LabTechnicianReadDto> CreateAsync(LabTechnicianCreateDto dto)
        {
            if (await _uow.LabTechnicians.GetByEmployeeIdAsync(dto.EmployeeId) is not null)
                throw new Exception("Employee ID already exists.");

            if (await _uow.LabTechnicians.GetByNationalIdAsync(dto.NationalId) is not null)
                throw new Exception("National ID already exists.");

            var entity = new LabTechnician
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Nationality = dto.Nationality,
                NationalId = dto.NationalId,

                EmployeeId = dto.EmployeeId,
                Laboratory = dto.Laboratory,
                JobTitle = dto.JobTitle,
                EmploymentStatus = dto.EmploymentStatus,
                WorkShift = dto.WorkShift,
                JoiningDate = dto.JoiningDate,
                YearsOfExperience = dto.YearsOfExperience,

                PhoneNumber = dto.PhoneNumber,
                AlternativePhone = dto.AlternativePhone,
                Email = dto.Email,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                PostalCode = dto.PostalCode,

                Username = dto.Username,
                AllowLogin = dto.AllowLogin,
                AccountActive = dto.AccountActive,
                ReceiveNotifications = dto.ReceiveNotifications,
                PhotoUrl = dto.PhotoUrl
            };

            await _uow.LabTechnicians.AddAsync(entity);

            return _mapper.Map<LabTechnicianReadDto>(entity);
        }

        public async Task<LabTechnicianReadDto> UpdateAsync(int id, LabTechnicianUpdateDto dto)
        {
            var entity = await _uow.LabTechnicians.GetByIdAsync(id)
                ?? throw new NotFoundException("LabTechnician", id);

            var employee = await _uow.LabTechnicians.GetByEmployeeIdAsync(dto.EmployeeId);
            if (employee != null && employee.Id != id)
                throw new Exception("Employee ID already exists.");

            var national = await _uow.LabTechnicians.GetByNationalIdAsync(dto.NationalId);
            if (national != null && national.Id != id)
                throw new Exception("National ID already exists.");

            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Gender = dto.Gender;
            entity.DateOfBirth = dto.DateOfBirth;
            entity.Nationality = dto.Nationality;
            entity.NationalId = dto.NationalId;

            entity.EmployeeId = dto.EmployeeId;
            entity.Laboratory = dto.Laboratory;
            entity.JobTitle = dto.JobTitle;
            entity.EmploymentStatus = dto.EmploymentStatus;
            entity.WorkShift = dto.WorkShift;
            entity.JoiningDate = dto.JoiningDate;
            entity.YearsOfExperience = dto.YearsOfExperience;

            entity.PhoneNumber = dto.PhoneNumber;
            entity.AlternativePhone = dto.AlternativePhone;
            entity.Email = dto.Email;
            entity.Address = dto.Address;
            entity.City = dto.City;
            entity.Country = dto.Country;
            entity.PostalCode = dto.PostalCode;

            entity.Username = dto.Username;
            entity.AllowLogin = dto.AllowLogin;
            entity.AccountActive = dto.AccountActive;
            entity.ReceiveNotifications = dto.ReceiveNotifications;
            entity.PhotoUrl = dto.PhotoUrl;

            await _uow.LabTechnicians.UpdateAsync(entity);

            return _mapper.Map<LabTechnicianReadDto>(entity);
        }

        public async Task<LabTechnicianReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.LabTechnicians.GetByIdAsync(id)
                ?? throw new NotFoundException("LabTechnician", id);

            return _mapper.Map<LabTechnicianReadDto>(entity);
        }

        public async Task<PaginatedResult<LabTechnicianReadDto>> GetAllAsync(LabTechnicianFilterDto filter)
        {
            var page = await _uow.LabTechnicians.SearchAsync(
                filter.Search,
                filter.Laboratory,
                filter.EmploymentStatus,
                filter.WorkShift,
                filter.JoiningDate,
                filter);

            return PaginatedResult<LabTechnicianReadDto>.Create(
                _mapper.Map<IEnumerable<LabTechnicianReadDto>>(page.Items),
                page.TotalCount,
                filter);
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _uow.LabTechnicians.ExistsAsync(id);

            if (!exists)
                throw new NotFoundException("LabTechnician", id);

            await _uow.LabTechnicians.SoftDeleteAsync(id);
        }
    }
}