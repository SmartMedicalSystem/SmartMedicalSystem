using Application.Common;
using Application.DTOs.AI;
using Application.DTOs.Patient;
using Application.DTOs.Patients;
using Application.Services.Abstraction;
using Application.Services.Abstraction.AI;
using Application.Services.AI;
using AutoMapper;
using Domain.Entities;
using Domain.IRepository;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _uow;
        private readonly Domain.IRepository.IPersonGenericRepo _personRepo;
        private readonly IMapper _mapper;
        private readonly IPatientResultAIService? _patientResultAIService;

        private readonly IMedicalAIClient? _aiClient;
        private readonly IRagService? _ragService;

        public PatientService(
            IUnitOfWork uow,
            Domain.IRepository.IPersonGenericRepo personRepo,
            IMapper mapper,
            IPatientResultAIService? patientResultAIService = null,
            IMedicalAIClient? aiClient = null,
            IRagService? ragService = null)
        {
            _uow = uow;
            _personRepo = personRepo;
            _mapper = mapper;
            _patientResultAIService = patientResultAIService;
            _aiClient = aiClient;
            _ragService = ragService;
        }

        public async Task<StoredFullReportDto?> GetStoredFullAIReportAsync(int patientId)
        {
            var doc = await _uow.PatientRagDocuments.GetByPatientAndSourceAsync(patientId, Domain.Enums.RagSourceType.FullPatientReport);
            if (doc == null)
                return null;

            return new StoredFullReportDto { Content = doc.Content };
        }

        public async Task UpdateStoredFullAIReportAsync(int patientId, string content, CancellationToken cancellationToken = default)
        {
            if (_ragService == null)
                throw new InvalidOperationException("RAG service not configured.");

            await _ragService.IndexAsync(patientId, null, Domain.Enums.RagSourceType.FullPatientReport, content, cancellationToken);
        }

        public async Task<PatientFullAIReportDto> GetFullAIReportAsync(int patientId, CancellationToken cancellationToken = default)
        {
            var patient = await _uow.Patients.GetByIdAsync(patientId)
                ?? throw new NotFoundException("Patient", patientId);
            if (_patientResultAIService == null || _aiClient == null || _ragService == null)
            {
                throw new InvalidOperationException("AI services not configured. Provide IPatientResultAIService, IMedicalAIClient and IRagService to use GetFullAIReportAsync.");
            }

            var patientResults = await _uow.PatientResults.GetByPatientAsync(patientId);

            var analyses = new List<PatientResultAIAnalysisDto>();
            foreach (var pr in patientResults)
            {
                // Reuse existing AI content when already generated; only call the model for
                // results that haven't been analyzed yet, so building the full report stays cheap
                // once the individual results have already been processed (e.g. by the MCP tools).
                var analysis = string.IsNullOrWhiteSpace(pr.Summary) || string.IsNullOrWhiteSpace(pr.AIClassifiedReport)
                    ? await _patientResultAIService.GenerateAnalysisAsync(pr.Id, cancellationToken)
                    : await _patientResultAIService.SummarizeElementsAsync(pr.Id, cancellationToken);

                analyses.Add(analysis);
            }

            var overallSummary = string.Empty;
            var overallSuggestion = string.Empty;

            if (analyses.Count > 0)
            {
                var userPrompt = PatientResultPromptBuilder.BuildFullReportUserPrompt(
                    patient.FullName, patient.Age, patient.Gender.ToString(), analyses);

                var raw = await _aiClient.GenerateAsync(
                    PatientResultPromptBuilder.FullReportSystemPrompt, userPrompt, cancellationToken);

                var parsed = PatientResultPromptBuilder.ParseJsonObject(raw);
                overallSummary = parsed.GetValueOrDefault("overallSummary") ?? parsed.GetValueOrDefault("raw") ?? string.Empty;
                overallSuggestion = parsed.GetValueOrDefault("overallSuggestion") ?? string.Empty;

                await _ragService.IndexAsync(patientId, null, Domain.Enums.RagSourceType.FullPatientReport,
                    $"Overall AI summary for {patient.FullName}: {overallSummary}\nOverall AI suggestion: {overallSuggestion}",
                    cancellationToken);
            }

            return new PatientFullAIReportDto
            {
                PatientId = patientId,
                PatientFullName = patient.FullName,
                Age = patient.Age,
                Gender = patient.Gender.ToString(),
                BloodType = patient.BloodType.ToString(),
                GeneratedAtUtc = DateTime.UtcNow,
                Results = analyses.OrderByDescending(a => a.GeneratedAtUtc).ToList(),
                OverallAISummary = overallSummary,
                OverallAISuggestion = overallSuggestion
            };
        }

        // Compatibility overloads for id-based operations
        public async Task<PatientReadDto> UpdateAsync(int id, PatientUpdateDto dto)
        {
            var entity = await _uow.Patients.GetByIdAsync(id)
                ?? throw new NotFoundException("Patient", id);

            entity.UpdateProfile(dto.FirstName, dto.LastName, dto.DateOfBirth);
            entity.UpdateInfo(dto.MobileNumber, dto.AlternativePhone, dto.Email, dto.Address, dto.City, dto.Country, dto.PostalCode, dto.BloodType);

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
                BloodType = dto.BloodType,
                 Email = dto.Email
            };
            await _personRepo.AddPerson(nationalId, entity);
            return _mapper.Map<PatientReadDto>(entity);
        }

        public async Task<PatientReadDto> UpdateAsync(string ssn, PatientUpdateDto dto)
        {
            var entity = await _personRepo.FindBySSN(ssn) as Domain.Entities.Patient
                ?? throw new NotFoundException("Patient", ssn);

            entity.UpdateProfile(dto.FirstName, dto.LastName, dto.DateOfBirth);
            entity.UpdateInfo(dto.MobileNumber, dto.AlternativePhone, dto.Email, dto.Address, dto.City, dto.Country, dto.PostalCode, dto.BloodType);
            await _uow.Patients.UpdateAsync(entity);
            return _mapper.Map<PatientReadDto>(entity);
        }

        public async Task<PaginatedResult<PatientReadDto>> GetAllAsync(PatientFilterDto filter)
        {
            // filter نفسه من نوع PatientFilterParams أصلاً (بالوراثة)، فبيتبعت
            // مباشرة للـ Repository من غير أي mapping يدوي.
            var page = await _uow.Patients.GetFilteredPaginatedAsync(filter);

            return PaginatedResult<PatientReadDto>.Create(
                _mapper.Map<IEnumerable<PatientReadDto>>(page.Items),
                page.TotalCount,
                filter);
        }

        public async Task DeleteAsync(string ssn)
        {
            var person = await _personRepo.FindBySSN(ssn)
                ?? throw new NotFoundException("Patient", ssn);

            await _uow.Patients.SoftDeleteAsync(person.Id);
        }

        public async Task<PatientReadDto> GetBySSNAsync(string ssn)
        {
            var entity = await _personRepo.FindBySSN(ssn)
                as Domain.Entities.Patient
                ?? throw new NotFoundException("Patient", ssn);

            return _mapper.Map<PatientReadDto>(entity);
        }

        // (int-based members implemented above)
    }
}
