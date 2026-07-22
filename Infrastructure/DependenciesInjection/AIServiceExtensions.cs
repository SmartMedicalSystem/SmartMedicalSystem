using Application.Services.Abstraction.AI;
using Infrastructure.Services.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.DependenciesInjection
{
    /// <summary>
    /// Wires up the AI client used for PatientResult summarization/report generation and for the
    /// RAG chatbot. Call from Program.cs, e.g.:
    ///   builder.Services.AddMedGemmaAI(builder.Configuration);
    /// </summary>
    public static class AIServiceExtensions
    {
        public static IServiceCollection AddMedGemmaAI(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MedGemmaOptions>(configuration.GetSection("AI"));

            services.AddHttpClient<IMedicalAIClient, MedGemmaAIClient>((provider, client) =>
            {
                var options = configuration.GetSection("AI").Get<MedGemmaOptions>() ?? new MedGemmaOptions();

                if (!string.IsNullOrWhiteSpace(options.BaseUrl))
                    client.BaseAddress = new Uri(options.BaseUrl);

                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            });

            return services;
        }
    }
}
