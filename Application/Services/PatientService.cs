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
        private readonly Domain.IRepository.IPersonGenericRepo _personRepo;
        private readonly IMapper _mapper;

        public PatientService(IUnitOfWork uow, Domain.IRepository.IPersonGenericRepo personRepo, IMapper mapper)
        {
            _uow = uow;
            _personRepo = personRepo;
            _mapper = mapper;
        }

        // Compatibility overloads for id-based operations
        public async Task<PatientReadDto> UpdateAsync(int id, PatientUpdateDto dto)
        {
            var entity = await _uow.Patients.GetByIdAsync(id)
                ?? throw new NotFoundException("Patient", id);

            entity.UpdateProfile(dto.FirstName, dto.LastName, dto.DateOfBirth);
            await _uow.Patients.UpdateAsync(entity);
            return _mapper.Map<PatientReadDto>(entity);
        }

        public async Task<PatientReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.Patients.GetByIdAsync(id)
                ?? throw new NotFoundException("Patient", id);
            return _mapper.Map<PatientReadDto>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _uow.Patients.SoftDeleteAsync(id);
        }

        public async Task<PatientReadDto> CreateAsync(PatientCreateDto dto)
        {
            var nationalId = dto.NationalId; // keep as string
            var entity = new Domain.Entities.Patient(dto.FirstName, dto.LastName, dto.DateOfBirth)
            {
                Gender = dto.Gender,
                PhoneNumber = dto.MobileNumber.ToString(),
                Address = dto.Address,
                BloodType = dto.BloodType
            };
            await _personRepo.AddPerson(nationalId, entity);
            return _mapper.Map<PatientReadDto>(entity);
        }

        public async Task<PatientReadDto> UpdateAsync(string ssn, PatientUpdateDto dto)
        {
            var entity = await _personRepo.FindBySSN(ssn) as Domain.Entities.Patient
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
            var person = await _personRepo.FindBySSN(ssn)
                ?? throw new NotFoundException("Patient", ssn);

            await _uow.Patients.SoftDeleteAsync(person.Id);
        }

        public async Task<PatientReadDto> GetBySSNAsync(string ssn)
        {
            
            var entity = await _personRepo.FindBySSN(ssn) as Domain.Entities.Patient
                ?? throw new NotFoundException("Patient", ssn);
            return _mapper.Map<PatientReadDto>(entity);
        }

        // (int-based members implemented above)
    }
}
