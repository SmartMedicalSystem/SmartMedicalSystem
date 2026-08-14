using Application.DTOs.AI;
using Application.DTOs.AIChat;
using Application.DTOs.Patient;
using Application.DTOs.Rag;
using Application.DTOs.Session;
using Application.DTOs.RequestLabs;
using Application.Services.Abstraction;
using Application.Services.Abstraction.AI;
using System.Text.Json;

namespace Application.Services.AI;

public sealed class PatientChatToolsService : IPatientChatToolsService
{
    private readonly IPatientService _patientService;
    private readonly IPatientResultService _patientResultService;
    private readonly IRagChatService _ragChatService;
    private readonly ISessionService _sessionService;
    private readonly IRequestLabsService _requestLabsService;
    private readonly IDoctorService _doctorService;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public PatientChatToolsService(
        IPatientService patientService,
        IPatientResultService patientResultService,
        IRagChatService ragChatService,
        ISessionService sessionService,
        IRequestLabsService requestLabsService,
        IDoctorService doctorService)
    {
        _patientService = patientService;
        _patientResultService = patientResultService;
        _ragChatService = ragChatService;
        _sessionService = sessionService;
        _requestLabsService = requestLabsService;
        _doctorService = doctorService;
    }

    public IReadOnlyList<AIChatToolDefinition> GetToolDefinitions() => new[]
    {
        Tool("get_patient", "Get a patient's demographic information by patient id.", "{\"type\":\"object\",\"properties\":{\"patientId\":{\"type\":\"integer\"}},\"required\":[\"patientId\"]}"),
        Tool("search_patients", "Search patients by name or national id. Returns a small result set for disambiguation; do not expose national ids to the user.", "{\"type\":\"object\",\"properties\":{\"search\":{\"type\":\"string\"}},\"required\":[\"search\"]}"),
        Tool("get_patient_full_ai_report", "Get the consolidated AI report for one patient, including the overall summary and per-result analyses.", "{\"type\":\"object\",\"properties\":{\"patientId\":{\"type\":\"integer\"}},\"required\":[\"patientId\"]}"),
        Tool("ask_patient_rag", "Ask the existing RAG chatbot a question about a patient's indexed clinical/lab history.", "{\"type\":\"object\",\"properties\":{\"patientId\":{\"type\":\"integer\"},\"question\":{\"type\":\"string\"},\"topK\":{\"type\":\"integer\"}},\"required\":[\"patientId\",\"question\"]}"),
        Tool("get_patient_result", "Get a single patient lab result by PatientResult id.", "{\"type\":\"object\",\"properties\":{\"patientResultId\":{\"type\":\"integer\"}},\"required\":[\"patientResultId\"]}"),
        Tool("generate_patient_result_ai_report", "Generate and persist an AI analysis for a PatientResult and index it into the existing RAG store. This changes server data and requires user confirmation.", "{\"type\":\"object\",\"properties\":{\"patientResultId\":{\"type\":\"integer\"}},\"required\":[\"patientResultId\"]}"),
        Tool("create_session", "Start a new clinical session/visit for a patient under the current doctor. Changes server data and requires user confirmation.", "{\"type\":\"object\",\"properties\":{\"patientId\":{\"type\":\"integer\"},\"notes\":{\"type\":\"string\"}},\"required\":[\"patientId\"]}"),
        Tool("create_lab_request", "Request one or more lab tests for an existing session. Changes server data and requires user confirmation.", "{\"type\":\"object\",\"properties\":{\"sessionId\":{\"type\":\"integer\"},\"labTestIds\":{\"type\":\"array\",\"items\":{\"type\":\"integer\"}},\"priority\":{\"type\":\"string\",\"enum\":[\"Low\",\"Normal\",\"High\",\"Urgent\"]}},\"required\":[\"sessionId\",\"labTestIds\"]}")
    };

    private static AIChatToolDefinition Tool(string name, string description, string schema) => new()
    {
        Name = name,
        Description = description,
        Parameters = JsonDocument.Parse(schema).RootElement.Clone()
    };

    public bool RequiresConfirmation(string toolName) =>
        toolName is "generate_patient_result_ai_report" or "create_session" or "create_lab_request";

    public string GetToolDescription(string toolName) => toolName switch
    {
        "generate_patient_result_ai_report" => "Generate and save an AI report for the selected lab result and index it in RAG.",
        "create_session" => "Start a new session for this patient under you.",
        "create_lab_request" => "Request the selected lab test(s) for this session.",
        _ => toolName
    };

    public async Task<string> ExecuteAsync(string toolName, JsonElement arguments, int? doctorId, CancellationToken cancellationToken = default)
    {
        return toolName switch
        {
            "get_patient" => await GetPatientAsync(arguments, cancellationToken),
            "search_patients" => await SearchPatientsAsync(arguments, cancellationToken),
            "get_patient_full_ai_report" => await GetFullReportAsync(arguments, cancellationToken),
            "ask_patient_rag" => await AskRagAsync(arguments, cancellationToken),
            "get_patient_result" => await GetPatientResultAsync(arguments, cancellationToken),
            "generate_patient_result_ai_report" => await GenerateReportAsync(arguments, cancellationToken),
            "create_session" => await CreateSessionAsync(arguments, doctorId, cancellationToken),
            "create_lab_request" => await CreateLabRequestAsync(arguments, cancellationToken),
            _ => throw new InvalidOperationException($"Unknown AI tool '{toolName}'.")
        };
    }

    private async Task<string> CreateSessionAsync(JsonElement args, int? doctorId, CancellationToken ct)
    {
        if (doctorId is null)
            return JsonSerializer.Serialize(new { error = "Could not determine the current doctor for this chat request." }, JsonOptions);

        var patientId = RequiredInt(args, "patientId");
        var notes = args.TryGetProperty("notes", out var n) ? n.GetString() : null;
        var doctor = await _doctorService.GetByIdAsync(doctorId.Value);

        var session = await _sessionService.CreateAsync(new SessionCreateDto
        {
            PatientId = patientId,
            DoctorId = doctorId.Value,
            DeptId = doctor.DepartmentId,
            SessionDate = DateTime.UtcNow,
            Notes = notes
        });

        return JsonSerializer.Serialize(new { session.Id, session.PatientId, message = "Session created." }, JsonOptions);
    }

    private async Task<string> CreateLabRequestAsync(JsonElement args, CancellationToken ct)
    {
        var sessionId = RequiredInt(args, "sessionId");
        var labTestIds = new List<int>();
        if (args.TryGetProperty("labTestIds", out var arr) && arr.ValueKind == JsonValueKind.Array)
            foreach (var item in arr.EnumerateArray())
                if (item.TryGetInt32(out var id)) labTestIds.Add(id);

        if (labTestIds.Count == 0)
            throw new ArgumentException("At least one labTestId is required.");

        var priority = Domain.Enums.LabRequestPriority.Normal;
        if (args.TryGetProperty("priority", out var p) && Enum.TryParse<Domain.Enums.LabRequestPriority>(p.GetString(), true, out var parsed))
            priority = parsed;

        var request = await _requestLabsService.CreateAsync(new RequestLabsCreateDto
        {
            SessionId = sessionId,
            RequestedAt = DateTime.UtcNow,
            LabTestIds = labTestIds,
            Priority = priority
        });

        return JsonSerializer.Serialize(new { request.Id, request.SessionId, message = "Lab request created." }, JsonOptions);
    }

    private async Task<string> GetPatientAsync(JsonElement args, CancellationToken ct)
    {
        var patientId = RequiredInt(args, "patientId");
        var patient = await _patientService.GetByIdAsync(patientId);
        return JsonSerializer.Serialize(new
        {
            patient.Id,
            patient.FirstName,
            patient.LastName,
            patient.Age,
            patient.DateOfBirth,
            Gender = patient.Gender.ToString(),
            patient.MobileNumber,
            patient.Address,
            BloodType = patient.BloodType.ToString()
        }, JsonOptions);
    }

    private async Task<string> SearchPatientsAsync(JsonElement args, CancellationToken ct)
    {
        var search = RequiredString(args, "search");
        var result = await _patientService.GetAllAsync(new Application.DTOs.Patients.PatientFilterDto
        {
            Search = search,
            PageNumber = 1,
            PageSize = 10
        });

        return JsonSerializer.Serialize(result.Items.Select(p => new
        {
            p.Id,
            p.FirstName,
            p.LastName,
            p.Age,
            Gender = p.Gender.ToString()
        }), JsonOptions);
    }

    private async Task<string> GetFullReportAsync(JsonElement args, CancellationToken ct)
    {
        var patientId = RequiredInt(args, "patientId");
        var report = await _patientService.GetFullAIReportAsync(patientId, ct);
        return JsonSerializer.Serialize(report, JsonOptions);
    }

    private async Task<string> AskRagAsync(JsonElement args, CancellationToken ct)
    {
        var patientId = RequiredInt(args, "patientId");
        var question = RequiredString(args, "question");
        var topK = args.TryGetProperty("topK", out var topKElement) && topKElement.TryGetInt32(out var k)
            ? Math.Clamp(k, 1, 20)
            : 5;

        var response = await _ragChatService.AskAsync(new RagChatRequestDto
        {
            PatientId = patientId,
            Question = question,
            TopK = topK
        }, ct);

        return JsonSerializer.Serialize(new { response.Answer, response.Sources }, JsonOptions);
    }

    private async Task<string> GetPatientResultAsync(JsonElement args, CancellationToken ct)
    {
        var patientResultId = RequiredInt(args, "patientResultId");
        var result = await _patientResultService.GetByIdAsync(patientResultId);
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    private async Task<string> GenerateReportAsync(JsonElement args, CancellationToken ct)
    {
        var patientResultId = RequiredInt(args, "patientResultId");
        var result = await _patientResultService.GenerateAIAnalysisAsync(patientResultId, ct);
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    private static int RequiredInt(JsonElement args, string name)
    {
        if (!args.TryGetProperty(name, out var value) || !value.TryGetInt32(out var result) || result <= 0)
            throw new ArgumentException($"'{name}' must be a positive integer.");
        return result;
    }

    private static string RequiredString(JsonElement args, string name)
    {
        if (!args.TryGetProperty(name, out var value) || value.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(value.GetString()))
            throw new ArgumentException($"'{name}' is required.");
        return value.GetString()!.Trim();
    }
}
