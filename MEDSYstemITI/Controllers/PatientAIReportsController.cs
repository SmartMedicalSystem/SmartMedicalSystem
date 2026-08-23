using Application.DTOs.AI;
using Application.DTOs.PatientResult;
using Application.Services.Abstraction;
using Application.Services.Auth;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MEDSYstemITI.Controllers
{
    /// <summary>
    /// AI-powered endpoints layered on top of PatientResult/Patient: generating the per-result
    /// summary/classified-report/suggestion, and the single consolidated report a doctor reads for
    /// a whole patient. The underlying generation pipeline is shared with the MCP tools in
    /// Application/MCPTools/PatientAIMcpTools.cs.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class PatientAIReportsController : ControllerBase
    {
        private readonly IPatientResultService _patientResultService;
        private readonly IPatientService _patientService;

        public PatientAIReportsController(IPatientResultService patientResultService, IPatientService patientService)
        {
            _patientResultService = patientResultService;
            _patientService = patientService;
        }

        /// <summary>
        /// Runs (or re-runs) AI summarization/classification/suggestion generation for a single
        /// PatientResult, persists it, and indexes it into the RAG vector store.
        /// </summary>
        [HttpPost("results/{patientResultId:int}/generate")]
        //[HasPermission(Permissions.CreateAiReport)]
        public async Task<ActionResult<PatientResultAIAnalysisDto>> GenerateForResult(
            int patientResultId, CancellationToken cancellationToken)
        {
            var result = await _patientResultService.GenerateAIAnalysisAsync(patientResultId, cancellationToken);
            return Ok(result);
        }

        [HttpPut("patients/{patientResultId:int}/results/update")]

        public async Task<ActionResult<PatientResultAIAnalysisDto>> UpdateForResult(
            int patientResultId, [FromBody] PatientResultUpdateDto updatedAnalysis, CancellationToken cancellationToken)
        {
            var result = await _patientResultService.UpdateAsync(patientResultId, updatedAnalysis, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// One consolidated report combining every AI summary/classified-report/suggestion
        /// generated for this patient's PatientResults, plus an AI-synthesized cross-result
        /// overview - the report a doctor should read/view for the patient as a whole.
        /// </summary>
        [HttpGet("patients/{patientId:int}/full-report")]
        //[HasPermission(Permissions.ReadAiReport)]
        public async Task<ActionResult<PatientFullAIReportDto>> GetFullPatientReport(
            int patientId, CancellationToken cancellationToken)
        {
            var report = await _patientService.GetFullAIReportAsync(patientId, cancellationToken);
            return Ok(report);
        }

        /// <summary>
        /// Returns the stored FullPatientReport document content from the RAG store if one exists.
        /// </summary>
        [HttpGet("patients/{patientId:int}/full-report/stored")]
        public async Task<ActionResult<Application.DTOs.AI.StoredFullReportDto?>> GetStoredFullPatientReport(int patientId)
        {
            var stored = await _patientService.GetStoredFullAIReportAsync(patientId);
            if (stored == null)
                return NotFound();

            return Ok(stored);
        }

        /// <summary>
        /// Stores or replaces the FullPatientReport content in the RAG store. This will delete
        /// any previous patient-level FullPatientReport chunks and index the provided content.
        /// </summary>
        [HttpPut("patients/{patientId:int}/full-report")]
        //[HasPermission(Permissions.UpdateAiReport)]
        public async Task<IActionResult> UpdateStoredFullPatientReport(int patientId, [FromBody] Application.DTOs.AI.StoredFullReportDto dto, CancellationToken cancellationToken)
        {
            await _patientService.UpdateStoredFullAIReportAsync(patientId, dto.Content, cancellationToken);
            return NoContent();
        }


    }
}
