using Application.DTOs.Patient;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork unit;
        private readonly IMapper mapper;

        public PatientService(IUnitOfWork _unit, IMapper _mapper)
        {
            unit = _unit;
            mapper = _mapper;
        }

        public async Task<IEnumerable<PatientDTO>> GetAllAsync()
        {
            var patients = await unit.Patients.GetAllAsync();

            return mapper.Map<IEnumerable<PatientDTO>>(patients);
        }

        public async Task<PatientDTO?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var patient = await unit.Patients.GetByIdAsync(id);

            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");

            return mapper.Map<PatientDTO>(patient);
        }
        public async Task<PatientDTO?> GetBySSNAsync(string ssn)
        {
            if(string.IsNullOrEmpty(ssn)) throw new ArgumentNullException(nameof(ssn));
            if (!ssn.All(char.IsDigit))
            {
                throw new ArgumentException("SSN must contain only digits.", nameof(ssn));
            }
            var Patient = await unit.Patients.GetBySSNAsync(ssn);
            if (Patient == null) throw new KeyNotFoundException("Patient not found.");

            return mapper.Map<PatientDTO>(Patient);
        }

        public async Task CreateAsync(CreatePatientDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var patient = mapper.Map<Patient>(dto);

            await unit.Patients.AddAsync(patient);

            await unit.SaveChangesAsync();
        }

        public async Task UpdateAsync(UpdatePatientDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var patient = await unit.Patients.GetByIdAsync(dto.Id);

            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");

            patient.Update
                (
                dto.FirstName,
                dto.LastName,
                dto.Contact,
                dto.Address
                )
                ;

            unit.Patients.Update(patient);

            await unit.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var patient = await unit.Patients.GetByIdAsync(id);

            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");

            unit.Patients.Delete(patient);

            await unit.SaveChangesAsync();
        }
    }
}