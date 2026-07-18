using Application.Common;
using Application.DTOs.LabTechnician;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.Entities;
using Domain.IRepository;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Application.Services
{
    public class LabTechnicianService : ILabTechnicianService
    {
        private readonly IUnitOfWork _uow;
        private readonly Domain.IRepository.IPersonGenericRepo _personRepo;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;

        public LabTechnicianService(IUnitOfWork uow, Domain.IRepository.IPersonGenericRepo personRepo, IMapper mapper , IFileStorageService fileStorageService)
        {
            _uow = uow;
            _personRepo = personRepo;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        // Compatibility overloads for id-based operations
        public async Task<LabTechnicianReadDto> UpdateAsync(int id, LabTechnicianUpdateDto dto)
        {
            var entity = await _uow.LabTechnicians.GetByIdAsync(id)
                ?? throw new NotFoundException("LabTechnician", id);

            // Update fields similar to SSN-based update
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Gender = dto.Gender;
            entity.DateOfBirth = dto.DateOfBirth.ToDateTime(System.TimeOnly.MinValue);
            entity.Nationality = dto.Nationality;

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
            // If a new photo file was provided, save it and update URL; otherwise keep existing
            if (dto.PhotoUrl != null)
            {
                var updatedPhotoUrl = await _fileStorageService.SaveImageAsync(dto.PhotoUrl);
                entity.PhotoUrl = updatedPhotoUrl ?? entity.PhotoUrl;
            }

            await _uow.LabTechnicians.UpdateAsync(entity);
            return _mapper.Map<LabTechnicianReadDto>(entity);
        }

        public async Task<LabTechnicianReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.LabTechnicians.GetByIdAsync(id)
                ?? throw new NotFoundException("LabTechnician", id);
            return _mapper.Map<LabTechnicianReadDto>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _uow.LabTechnicians.SoftDeleteAsync(id);
        }

        public async Task<LabTechnicianReadDto> CreateAsync(LabTechnicianCreateDto dto)
        {
            //if (await _uow.LabTechnicians.GetByEmployeeIdAsync(dto.EmployeeId) is not null)
                //throw new Exception("Employee ID already exists.");

            if (await _uow.LabTechnicians.GetByNationalIdAsync(dto.NationalId) is not null)
                throw new Exception("National ID already exists.");

            var photoUrl = await _fileStorageService.SaveImageAsync(dto.PhotoUrl);

            var entity = new LabTechnician
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth.ToDateTime(System.TimeOnly.MinValue),
                Nationality = dto.Nationality,

                //employeeidentitynumber = dto.employeeidentitynumber,
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
                PhotoUrl = photoUrl,

                Username = dto.Username,
                AllowLogin = dto.AllowLogin,
                AccountActive = dto.AccountActive,
                ReceiveNotifications = dto.ReceiveNotifications,
                
            };

            await _uow.LabTechnicians.AddAsync(entity);

            return _mapper.Map<LabTechnicianReadDto>(entity);
        }

        public async Task<LabTechnicianReadDto> UpdateAsync(string ssn, LabTechnicianUpdateDto dto)
        {
            var person = await _personRepo.FindBySSN(ssn)
                ?? throw new NotFoundException("LabTechnician", ssn);

            // Locate the lab technician entity using the person id
            var entity = await _uow.LabTechnicians.GetByIdAsync(person.Id)
                ?? throw new NotFoundException("LabTechnician", person.Id);

            // If provided, save new photo
            if (dto.PhotoUrl != null)
            {
                var photoUrl = await _fileStorageService.SaveImageAsync(dto.PhotoUrl);
                entity.PhotoUrl = photoUrl ?? entity.PhotoUrl;
            }

            var national = await _uow.LabTechnicians.GetByNationalIdAsync(dto.NationalId);
            if (national != null && national.Id != entity.Id)
                throw new Exception("National ID already exists.");

            // Update allowed profile fields
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Gender = dto.Gender;
            entity.DateOfBirth = dto.DateOfBirth.ToDateTime(System.TimeOnly.MinValue);
            entity.Nationality = dto.Nationality;
            // Update encrypted national id via person repo so it's stored consistently
            await _personRepo.UpdateSSNAsync(entity, dto.NationalId);

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

            await _uow.LabTechnicians.UpdateAsync(entity);

            return _mapper.Map<LabTechnicianReadDto>(entity);
        }

        public async Task<LabTechnicianReadDto> GetBySSNAsync(string ssn)
        {
            var entity = await _personRepo.FindBySSN(ssn)
                ?? throw new NotFoundException("LabTechnician", ssn);
            return _mapper.Map<LabTechnicianReadDto>(entity);
        }

        public async Task<PaginatedResult<LabTechnicianReadDto>> GetAllAsync(PaginationParams pagination)
        {
            var page = await _uow.LabTechnicians.GetAllActivePaginatedAsync(pagination);
            return PaginatedResult<LabTechnicianReadDto>.Create(
                _mapper.Map<IEnumerable<LabTechnicianReadDto>>(page.Items),
                page.TotalCount, pagination);
        }

        // Additional overload used by some callers that accept a filter DTO
        public async Task<PaginatedResult<LabTechnicianReadDto>> GetAllAsync(Application.DTOs.LabTechnician.LabTechnicianFilterDto filter)
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
                page.TotalCount, filter);
        }

        public async Task DeleteAsync(string ssn)
        {
            var person = await _personRepo.FindBySSN(ssn)
                ?? throw new NotFoundException("LabTechnician", ssn);

            await _uow.LabTechnicians.SoftDeleteAsync(person.Id);
        }

        // (int-based overloads implemented above)



      
    }
}