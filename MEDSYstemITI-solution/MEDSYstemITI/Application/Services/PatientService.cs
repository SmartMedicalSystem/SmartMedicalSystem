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
            var nationalId = int.TryParse(dto.NationalId, out var nid) ? nid : throw new System.ArgumentException("Invalid NationalId", nameof(dto.NationalId));
            var entity = new Domain.Entities.Patient(dto.FirstName, dto.LastName, nationalId, dto.DateOfBirth, dto.Gender, dto.MobileNumber, dto.Address, dto.BloodType);
            await _uow.PersonGeneric.AddPerson(dto.NationalId, entity);
            return _mapper.Map<PatientReadDto>(entity);
        }

        public async Task<PatientReadDto> UpdateAsync(string ssn, PatientUpdateDto dto)
        {
            var entity = await _uow.PersonGeneric.FindBySSN(ssn) as Domain.Entities.Patient
                ?? throw new NotFoundException("Patient", ssn);

            entity.UpdateProfile(dto.FirstName, dto.LastName, dto.DateOfBirth);
            await _uow.Patients.UpdateAsync(entity);
            return _mapper.Map<PatientReadDto>(entity);
        }

        public async Task<PaginatedResult<PatientReadDto>> GetAllAsync(PaginationParams pagination)
        {
            var page = await _uow.Patients.GetAllActivePaginatedAsync(pagination);
            return PaginatedResult<PatientReadDto>.Create(
                _mapper.Map<IEnumerable<PatientReadDto>>(page.Items),
                page.TotalCount, pagination);
        }

        public async Task DeleteAsync(string ssn)
        {
            var person = await _uow.PersonGeneric.FindBySSN(ssn)
                ?? throw new NotFoundException("Patient", ssn);

            await _uow.Patients.SoftDeleteAsync(person.Id);
        }

        public async Task<PatientReadDto> GetBySSNAsync(string ssn)
        {
            
            var entity = await _uow.PersonGeneric.FindBySSN(ssn) as Domain.Entities.Patient
                ?? throw new NotFoundException("Patient", ssn);
            return _mapper.Map<PatientReadDto>(entity);
        }
    }
}
