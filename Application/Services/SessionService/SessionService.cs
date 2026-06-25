using Application.DTOs.SessionDto;
using Application.Interfaces.SessionServiceAbstract;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.SessionService
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _repository;

        public SessionService(ISessionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SessionDto>> GetAllAsync()
        {
            var sessions = await _repository.GetAllAsync();

            return sessions.Select(s => new SessionDto
            {
                SessionId = s.SessionId,
                PatientName = $"{s.Patient.FirstName} {s.Patient.LastName}",
                DoctorName = $"{s.Doctor.FirstName} {s.Doctor.LastName}",
                DepartmentName = s.Department.Name,
                SessionDate = s.SessionDate,
                Notes = s.Notes
            });
        }

        public async Task<SessionDto?> GetByIdAsync(int id)
        {
            var session = await _repository.GetByIdAsync(id);

            if (session == null)
                return null;

            return new SessionDto
            {
                SessionId = session.SessionId,
                PatientName = $"{session.Patient.FirstName} {session.Patient.LastName}",
                DoctorName = $"{session.Doctor.FirstName} {session.Doctor.LastName}",
                DepartmentName = session.Department.Name,
                SessionDate = session.SessionDate,
                Notes = session.Notes
            };
        }

        public async Task CreateAsync(CreateSessionDto dto)
        {
            var session = new Session
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                DeptId = dto.DeptId,
                SessionDate = dto.SessionDate,
                Notes = dto.Notes
            };

            await _repository.AddAsync(session);
        }

        public async Task UpdateAsync(UpdateSessionDto dto)
        {
            var session = await _repository.GetByIdAsync(dto.SessionId);

            if (session == null)
                throw new Exception("Session not found");

            session.PatientId = dto.PatientId;
            session.DoctorId = dto.DoctorId;
            session.DeptId = dto.DeptId;
            session.SessionDate = dto.SessionDate;
            session.Notes = dto.Notes;

            await _repository.UpdateAsync(session);
        }

        public async Task DeleteAsync(int id)
        {
            var session = await _repository.GetByIdAsync(id);

            if (session == null)
                throw new Exception("Session not found");

            await _repository.DeleteAsync(session);
        }

    }
}
