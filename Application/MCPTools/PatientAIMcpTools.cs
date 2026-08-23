using Application.DTOs.Rag;
using Application.Services.Abstraction;
using Application.Services.Abstraction.AI;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Application.MCPTools
{
    /// <summary>
    /// MCP tools exposing the system's AI capabilities to any MCP-compatible client/agent
    /// (Claude Desktop, Claude Code, an internal copilot, etc.). Registered automatically by
    /// `builder.Services.AddMcpServer().WithToolsFromAssembly()` in the host project's Program.cs,
    /// which scans this assembly for [McpServerToolType] classes.
    ///
    /// Every tool method delegates to the same Application-layer services used by the REST
    /// controllers (IPatientResultService / IPatientService / IRagChatService), so business logic,
    /// validation, and RAG indexing behave identically whether triggered via HTTP or via MCP.
    /// Tool constructors are resolved through DI per call, so scoped services (EF DbContext,
    /// UnitOfWork, ...) behave exactly as they would in a normal request.
    /// </summary>
    [McpServerToolType]
    public class PatientAIMcpTools
    {
        private readonly IPatientResultService _patientResultService;
        private readonly IPatientResultAIService _patientResultAIService;
        private readonly IPatientService _patientService;
        private readonly IRagChatService _ragChatService;
        private readonly IRagService _ragService;

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = false
        };

        public PatientAIMcpTools(
            IPatientResultService patientResultService,
            IPatientResultAIService patientResultAIService,
            IPatientService patientService,
            IRagChatService ragChatService,
            IRagService ragService)
        {
            _patientResultService = patientResultService;
            _patientResultAIService = patientResultAIService;
            _patientService = patientService;
            _ragChatService = ragChatService;
            _ragService = ragService;
        }

        [McpServerTool(Name = "summarize_patient_result_elements"),
         Description("Deterministically compares every lab element in a PatientResult against its normal reference range " +
                     "(no AI call) and returns the breakdown as JSON. Useful for a quick, cost-free preview before generating " +
                     "the full AI report.")]
        public async Task<string> SummarizePatientResultElements(
            [Description("The PatientResult id to summarize.")] int patientResultId,
            CancellationToken cancellationToken = default)
        {
            var summary = await _patientResultAIService.SummarizeElementsAsync(patientResultId, cancellationToken);
            return JsonSerializer.Serialize(summary, JsonOptions);
        }

        [McpServerTool(Name = "generate_patient_result_ai_report"),
         Description("Uses the configured AI model (MedGemma) to summarize a PatientResult's lab elements and generate " +
                     "its classified clinical report and physician suggestions. Persists the result onto the PatientResult " +
                     "record and indexes the generated text into the RAG vector store for the chatbot. Returns the full " +
                     "analysis as JSON.")]
        public async Task<string> GeneratePatientResultAIReport(
            [Description("The PatientResult id to analyze.")] int patientResultId,
            CancellationToken cancellationToken = default)
        {
            var analysis = await _patientResultService.GenerateAIAnalysisAsync(patientResultId, cancellationToken);
            return JsonSerializer.Serialize(analysis, JsonOptions);
        }

        [McpServerTool(Name = "get_patient_full_ai_report"),
         Description("Gathers every AI summary/classified-report/suggestion generated for a patient's PatientResults into " +
                     "one consolidated report, plus one AI-synthesized cross-result overview - the same report a doctor " +
                     "sees in the front-end. Generates AI analysis on the fly for any result that doesn't have it yet.")]
        public async Task<string> GetPatientFullAIReport(
            [Description("The Patient id to build the consolidated report for.")] int patientId,
            CancellationToken cancellationToken = default)
        {
            var report = await _patientService.GetFullAIReportAsync(patientId, cancellationToken);
            return JsonSerializer.Serialize(report, JsonOptions);
        }

        [McpServerTool(Name = "ask_patient_rag_chatbot"),
         Description("Answers a natural-language question about a patient's lab history using Retrieval-Augmented " +
                     "Generation over previously AI-indexed summaries/reports/suggestions for that patient. Returns the " +
                     "answer together with the source chunks used, as JSON. don't mention patient's id")]
        public async Task<string> AskPatientRagChatbot(
            [Description("The physician's question, e.g. 'has the patient's kidney function been trending down?'")] string question,
            [Description("Optional: restrict retrieval to this patient's indexed documents only. Recommended for most questions.")] int? patientId = null,
            [Description("How many top-matching passages to retrieve as context. Default 5.")] int topK = 5,
            CancellationToken cancellationToken = default)
        {
            var response = await _ragChatService.AskAsync(new RagChatRequestDto
            {
                Question = question,
                PatientId = patientId,
                TopK = topK
            }, cancellationToken);

            return JsonSerializer.Serialize(response, JsonOptions);
        }

        [McpServerTool(Name = "index_patient_note_for_rag"),
         Description("Manually embeds and indexes an arbitrary piece of clinical text (e.g. a doctor's free-text note) " +
                     "into a patient's RAG vector store so the chatbot can retrieve it in future answers.")]
        public async Task<string> IndexPatientNoteForRag(
            [Description("The Patient id this note belongs to.")] int patientId,
            [Description("The note text to embed and index.")] string content,
            CancellationToken cancellationToken = default)
        {
            await _ragService.IndexAsync(patientId, null, Domain.Enums.RagSourceType.Manual, content, cancellationToken);
            return JsonSerializer.Serialize(new { indexed = true, patientId }, JsonOptions);
        }
    }
}
