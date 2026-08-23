using Application.Common;
using Application.DTOs.AI;
using Application.Services.Abstraction.AI;
using Domain.Entities;
using Domain.IRepository;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.AI
{
    public class PatientResultAIService : IPatientResultAIService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMedicalAIClient _aiClient;
        private readonly IRagService _ragService;

        public PatientResultAIService(IUnitOfWork uow, IMedicalAIClient aiClient, IRagService ragService)
        {
            _uow = uow;
            _aiClient = aiClient;
            _ragService = ragService;
        }

        public async Task<PatientResultAIAnalysisDto> SummarizeElementsAsync(int patientResultId, CancellationToken cancellationToken = default)
        {
            var patientResult = await _uow.PatientResults.GetWithResultElementsAsync(patientResultId)
                ?? throw new NotFoundException("PatientResult", patientResultId);

            var dto = BuildBaseDto(patientResult);

            // If the patient result already contains a summary/report/suggestion, ensure it's
            // indexed into the RAG vector store so retrieval-based QA can find it later.
            if (!string.IsNullOrWhiteSpace(dto.Summary))
            {
                await _ragService.IndexAsync(dto.PatientId, dto.PatientResultId, RagSourceType.ResultSummary,
                    $"[{dto.LabTestName}] Summary: {dto.Summary}", cancellationToken);
            }
            if (!string.IsNullOrWhiteSpace(dto.AIClassifiedReport))
            {
                await _ragService.IndexAsync(dto.PatientId, dto.PatientResultId, RagSourceType.ResultReport,
                    $"[{dto.LabTestName}] Classified report: {dto.AIClassifiedReport}", cancellationToken);
            }
            if (!string.IsNullOrWhiteSpace(dto.AISuggestion))
            {
                await _ragService.IndexAsync(dto.PatientId, dto.PatientResultId, RagSourceType.ResultSuggestion,
                    $"[{dto.LabTestName}] Suggestion: {dto.AISuggestion}", cancellationToken);
            }

            return dto;
        }

        public async Task<PatientResultAIAnalysisDto> GenerateAnalysisAsync(int patientResultId, CancellationToken cancellationToken = default)
        {
            var patientResult = await _uow.PatientResults.GetWithResultElementsAsync(patientResultId)
                ?? throw new NotFoundException("PatientResult", patientResultId);

            var dto = BuildBaseDto(patientResult);

            var userPrompt = PatientResultPromptBuilder.BuildResultAnalysisUserPrompt(dto.LabTestName, dto.Elements);
            var rawResponse = await _aiClient.GenerateAsync(
                PatientResultPromptBuilder.ResultAnalysisSystemPrompt, userPrompt, cancellationToken);

            var parsed = PatientResultPromptBuilder.ParseJsonObject(rawResponse);

            var summary = parsed.GetValueOrDefault("summary")
                ?? parsed.GetValueOrDefault("raw")
                ?? "AI summary unavailable.";
            var classifiedReport = parsed.GetValueOrDefault("classifiedReport") ?? string.Empty;
            var suggestion = parsed.GetValueOrDefault("suggestion") ?? string.Empty;

            // Persist onto the entity using its own validated mutator (never bypassed via AutoMapper - see MappingProfile).
            patientResult.UpdateAIOutput(classifiedReport, suggestion, Truncate(summary, 2000));
            await _uow.PatientResults.UpdateAsync(patientResult);

            dto.Summary = patientResult.Summary;
            dto.AIClassifiedReport = patientResult.AIClassifiedReport;
            dto.AISuggestion = patientResult.AISuggestion;
            dto.GeneratedAtUtc = DateTime.UtcNow;

            // Index each piece separately so the chatbot can retrieve the most relevant slice
            // (a question about "next steps" should match the suggestion chunk, not the summary).
            await _ragService.IndexAsync(dto.PatientId, dto.PatientResultId, RagSourceType.ResultSummary,
                $"[{dto.LabTestName}] Summary: {dto.Summary}", cancellationToken);
            await _ragService.IndexAsync(dto.PatientId, dto.PatientResultId, RagSourceType.ResultReport,
                $"[{dto.LabTestName}] Classified report: {dto.AIClassifiedReport}", cancellationToken);
            await _ragService.IndexAsync(dto.PatientId, dto.PatientResultId, RagSourceType.ResultSuggestion,
                $"[{dto.LabTestName}] Suggestion: {dto.AISuggestion}", cancellationToken);

            return dto;
        }

        private static PatientResultAIAnalysisDto BuildBaseDto(PatientResult patientResult)
        {
            var elements = patientResult.ResultElements
                .Where(e => !e.IsDeleted)
                .OrderBy(e => e.TestElement.ElementName)
                .Select(e => new PatientResultElementSummaryDto
                {
                    TestElementId = e.TestElementId,
                    ElementName = e.TestElement.ElementName,
                    Unit = e.TestElement.Unit,
                    Value = e.Value,
                    NormalMin = e.TestElement.NormalMin,
                    NormalMax = e.TestElement.NormalMax,
                    Flag = ClassifyFlag(e.Value, e.TestElement.NormalMin, e.TestElement.NormalMax)
                })
                .ToList();

            return new PatientResultAIAnalysisDto
            {
                PatientResultId = patientResult.Id,
                PatientId = patientResult.PatientId,
                SessionId = patientResult.SessionId,
                LabTestId = patientResult.LabTestId,
                LabTestName = patientResult.labTest?.TestName ?? $"LabTest #{patientResult.LabTestId}",
                GeneratedAtUtc = DateTime.UtcNow,
                TestDate = patientResult.Session?.SessionDate ?? default,
                Elements = elements,
                Summary = patientResult.Summary,
                AIClassifiedReport = patientResult.AIClassifiedReport,
                AISuggestion = patientResult.AISuggestion
            };
        }

        private static string ClassifyFlag(double value, float min, float max)
        {
            if (value < min) return "Low";
            if (value > max) return "High";
            return "Normal";
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
                return value;

            return value[..maxLength];
        }
    }
}
