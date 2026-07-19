using Application.Common;
using Application.DTOs.Session;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<SessionReadDto> CreateAsync(SessionCreateDto dto)
        {
            // Business rule: patient, doctor and department referenced must all exist,
            // and the doctor must actually belong to the given department.
            _ = await _uow.Patients.GetByIdAsync(dto.PatientId) ?? throw new NotFoundException("Patient", dto.PatientId);
            var doctor = await _uow.Doctors.GetByIdAsync(dto.DoctorId) ?? throw new NotFoundException("Doctor", dto.DoctorId);
            _ = await _uow.Departments.GetByIdAsync(dto.DeptId) ?? throw new NotFoundException("Department", dto.DeptId);

            if (doctor.DepartmentId != dto.DeptId)
                throw new System.ArgumentException("The selected doctor does not belong to the selected department.");

            var entity = new Domain.Entities.Session(dto.PatientId, dto.DoctorId, dto.DeptId, dto.SessionDate, dto.Notes);
            await _uow.Sessions.AddAsync(entity);
            return _mapper.Map<SessionReadDto>(entity);
        }

        public async Task<SessionReadDto> UpdateAsync(int id, SessionUpdateDto dto)
        {
            var entity = await _uow.Sessions.GetByIdAsync(id)
                ?? throw new NotFoundException("Session", id);

            entity.Reschedule(dto.SessionDate);
            entity.UpdateNotes(dto.Notes);
            await _uow.Sessions.UpdateAsync(entity);
            return _mapper.Map<SessionReadDto>(entity);
        }

        public async Task<SessionReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.Sessions.GetByIdAsync(id)
                ?? throw new NotFoundException("Session", id);
            return _mapper.Map<SessionReadDto>(entity);
        }

        public async Task<PaginatedResult<SessionReadDto>> GetByPatientAsync(int patientId, PaginationParams pagination)
        {
            var page = await _uow.Sessions.GetByPatientPaginatedAsync(patientId, pagination);
            return PaginatedResult<SessionReadDto>.Create(
                _mapper.Map<IEnumerable<SessionReadDto>>(page.Items),
                page.TotalCount, pagination);
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _uow.Sessions.ExistsAsync(id);
            if (!exists) throw new NotFoundException("Session", id);
            await _uow.Sessions.SoftDeleteAsync(id);
        }
    }
}
