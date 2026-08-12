using Application.DTOs.AI;
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
            await _patientResultService.NotifyPatientAsync(result);
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
    }
}
