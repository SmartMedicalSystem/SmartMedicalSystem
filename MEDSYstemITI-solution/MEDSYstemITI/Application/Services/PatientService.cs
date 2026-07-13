using Application.Common;
using Application.DTOs.Patient;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PatientService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<PatientReadDto> CreateAsync(PatientCreateDto dto)
        {
            var entity = new Domain.Entities.Patient(dto.FirstName, dto.LastName,dto.NationalId, dto.DateOfBirth, dto.Gender, dto.MobileNumber, dto.Address, dto.BloodType);
            await _uow.Patients.AddAsync(entity);
            return _mapper.Map<PatientReadDto>(entity);
        }

        public async Task<PatientReadDto> UpdateAsync(int id, PatientUpdateDto dto)
        {
            var entity = await _uow.Patients.GetByIdAsync(id)
                ?? throw new NotFoundException("Patient", id);

            entity.UpdateProfile(dto.FirstName, dto.LastName, dto.NationalId, dto.DateOfBirth, dto.Gender, dto.MobileNumber, dto.Address, dto.BloodType);
            await _uow.Patients.UpdateAsync(entity);
            return _mapper.Map<PatientReadDto>(entity);
        }

        public async Task<PatientReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.Patients.GetByIdAsync(id)
                ?? throw new NotFoundException("Patient", id);
            return _mapper.Map<PatientReadDto>(entity);
        }

        public async Task<PaginatedResult<PatientReadDto>> GetAllAsync(PaginationParams pagination)
        {
            var page = await _uow.Patients.GetAllActivePaginatedAsync(pagination);
            return PaginatedResult<PatientReadDto>.Create(
                _mapper.Map<IEnumerable<PatientReadDto>>(page.Items),
                page.TotalCount, pagination);
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _uow.Patients.ExistsAsync(id);
            if (!exists) throw new NotFoundException("Patient", id);
            await _uow.Patients.SoftDeleteAsync(id);
        }
    }
}
