using Application.DTOs.PatientResultDtos;
using Application.DTOs.PatientResultElementDtos;
using Application.Interfaces.PatientResultServiceAbstract;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.PatientResultService
{
    public class PatientResultService : IPatientResultService
    {
        private readonly IPatientResultRepository _repository;

        public PatientResultService(IPatientResultRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PatientResultDto>> GetAllAsync()
        {
            var results = await _repository.GetAllAsync();

            return results.Select(r => new PatientResultDto
            {
                ResultId = r.ResultId,
                PatientName = $"{r.Patient.FirstName} {r.Patient.LastName}",
                TestName = r.Test.Name,
                ResultDate = r.ResultDate,
                AIClassifiedReport = r.AIClassifiedReport,
                AISuggestion = r.AISuggestion,
                Summary = r.Summary,
                Elements = r.ResultElements.Select(e => new PatientResultElementDetailsDto
                {
                    ElementName = e.TestElement.Name,
                    Value = e.Value,
                    TechnicianName = $"{e.Technician.FirstName} {e.Technician.LastName}"
                }).ToList()
            });
        }

        public async Task<PatientResultDto?> GetByIdAsync(int id)
        {
            var r = await _repository.GetByIdAsync(id);

            if (r == null)
                return null;

            return new PatientResultDto
            {
                ResultId = r.ResultId,
                PatientName = $"{r.Patient.FirstName} {r.Patient.LastName}",
                TestName = r.Test.Name,
                ResultDate = r.ResultDate,
                AIClassifiedReport = r.AIClassifiedReport,
                AISuggestion = r.AISuggestion,
                Summary = r.Summary,
                Elements = r.ResultElements.Select(e => new PatientResultElementDetailsDto
                {
                    ElementName = e.TestElement.Name,
                    Value = e.Value,
                    TechnicianName = $"{e.Technician.FirstName} {e.Technician.LastName}"
                }).ToList()
            };
        }

        public async Task CreateAsync(CreatePatientResultDto dto)
        {
            var result = new PatientResult
            {
                PatientId = dto.PatientId,
                SessionId = dto.SessionId,
                TestId = dto.TestId,
                AIClassifiedReport = dto.AIClassifiedReport,
                AISuggestion = dto.AISuggestion,
                Summary = dto.Summary,
                ResultDate = DateTime.UtcNow,
                ResultElements = dto.Elements.Select(x =>
                    new PatientResultElement
                    {
                        TestElementId = x.TestElementId,
                        Value = x.Value,
                        TechId = x.TechId
                    }).ToList()
            };

            await _repository.AddAsync(result);
        }

        public async Task DeleteAsync(int id)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                throw new Exception("Result not found");

            await _repository.DeleteAsync(result);
        }
    }
}
