using Application.Mapping;
using Application.Services;
using Application.Services.Abstraction;
using Application.Services.Abstraction.AI;
using Application.Services.Abstraction.Auth;
using Application.Services.AI;
using Application.Services.Auth;
using Domain.IRepository;
using MEDSYstemITI.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection
{
    /// <summary>
    /// Wires up AutoMapper and every business-logic service in the Application
    /// layer. Call this once from the API project's Program.cs, e.g.:
    ///   builder.Services.AddApplicationServices();
    /// </summary>
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<ILabTestService, LabTestService>();
            services.AddScoped<ITestElementService, TestElementService>();
            services.AddScoped<ILabTestElementService, LabTestElementService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IRequestLabsService, RequestLabsService>();
            services.AddScoped<INotificationService , NotificationService>();
            services.AddScoped<INotificationAppService, NotificationAppService>();
            services.AddScoped<ILabTechnicianService, LabTechnicianService>();
            services.AddScoped<IPatientResultService, PatientResultService>();
            services.AddScoped<IPatientResultElementService, PatientResultElementService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ILabTechProfileService, LabTechProfileService>();

            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            // AI / RAG services (IMedicalAIClient itself is registered by Infrastructure's
            // AddMedGemmaAI extension, called from Program.cs, since it needs an HttpClient).

            services.AddScoped<IPatientResultAIService, PatientResultAIService>();
            services.AddScoped<IRagService, RagService>();
            services.AddScoped<IRagChatService, RagChatService>();


            return services;
        }
    }
}
