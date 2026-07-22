using Application.Common;
using Application.DTOs.LabTechProfile;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LabTechProfileService : ILabTechProfileService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public LabTechProfileService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<LabTechProfileReadDto> CreateAsync(LabTechProfileCreateDto dto)
        {
            if (await _uow.LabTechProfiles.ExistsForUserAsync(dto.UserId))
                throw new System.ArgumentException("This user already has a Lab Technician profile.");

            if (await _uow.LabTechProfiles.EmployeeIdExistsAsync(dto.EmployeeId))
                throw new System.ArgumentException("This Employee ID is already in use.");

            var entity = new Domain.Entities.LabTechProfile(
                dto.UserId,
                dto.FirstName,
                dto.LastName,
                dto.Gender,
                dto.DateOfBirth,
                dto.Nationality,
                dto.NationalId,
                dto.EmployeeId,
                dto.AssignedLaboratory,
                dto.JobTitle,
                dto.YearsOfExperience,
                dto.JoiningDate,
                dto.Email,
                dto.PhoneNumber,
                dto.Address);

            await _uow.LabTechProfiles.AddAsync(entity);
            return _mapper.Map<LabTechProfileReadDto>(entity);
        }

        public async Task<LabTechProfileReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.LabTechProfiles.GetByIdAsync(id)
                ?? throw new NotFoundException("LabTechProfile", id);
            return _mapper.Map<LabTechProfileReadDto>(entity);
        }

        public async Task<LabTechProfileReadDto> GetByUserIdAsync(int userId)
        {
            var entity = await _uow.LabTechnicians.GetByIdAsync(userId)
                ?? throw new NotFoundException("LabTechProfile", userId);
            return _mapper.Map<LabTechProfileReadDto>(entity);
        }

        public async Task<PaginatedResult<LabTechProfileReadDto>> GetAllPaginatedAsync(PaginationParams pagination)
        {
            var page = await _uow.LabTechProfiles.GetAllPaginatedAsync(pagination);
            return PaginatedResult<LabTechProfileReadDto>.Create(
                _mapper.Map<IEnumerable<LabTechProfileReadDto>>(page.Items),
                page.TotalCount, pagination);
        }

        public async Task<LabTechProfileReadDto> UpdateOwnProfileAsync(int userId, LabTechProfileUpdateDto dto)
        {
            var entity = await _uow.LabTechProfiles.GetByUserIdAsync(userId)
                ?? throw new NotFoundException("LabTechProfile", userId);

            entity.UpdatePersonalInfo(dto.FirstName, dto.LastName, dto.Gender, dto.DateOfBirth, dto.Nationality, dto.NationalId);
            entity.UpdateContactInfo(dto.Email, dto.PhoneNumber, dto.Address);

            await _uow.LabTechProfiles.UpdateAsync(entity);
            return _mapper.Map<LabTechProfileReadDto>(entity);
        }

        public async Task<LabTechProfileReadDto> UpdateProfilePictureAsync(int userId, LabTechProfilePictureDto dto)
        {
            var entity = await _uow.LabTechProfiles.GetByUserIdAsync(userId)
                ?? throw new NotFoundException("LabTechProfile", userId);

            entity.UpdateProfilePicture(dto.ProfilePictureUrl);

            await _uow.LabTechProfiles.UpdateAsync(entity);
            return _mapper.Map<LabTechProfileReadDto>(entity);
        }

        public async Task<LabTechProfileReadDto> UpdateProfessionalInfoAsync(int id, LabTechProfileAdminUpdateDto dto)
        {
            var entity = await _uow.LabTechProfiles.GetByIdAsync(id)
                ?? throw new NotFoundException("LabTechProfile", id);

            entity.UpdateProfessionalInfo(dto.AssignedLaboratory, dto.JobTitle, dto.YearsOfExperience);

            await _uow.LabTechProfiles.UpdateAsync(entity);
            return _mapper.Map<LabTechProfileReadDto>(entity);
        }

        public async Task<LabTechProfileReadDto> UpdateStatusAsync(int id, LabTechProfileStatusDto dto)
        {
            var entity = await _uow.LabTechProfiles.GetByIdAsync(id)
                ?? throw new NotFoundException("LabTechProfile", id);

            entity.UpdateStatus(dto.Status);

            await _uow.LabTechProfiles.UpdateAsync(entity);
            return _mapper.Map<LabTechProfileReadDto>(entity);
        }
    }
}
