using Application.DependencyInjection;
using Application.MCPTools;
using Application.Services.Abstraction;
using Domain.IRepository;
using Infrastructure.DataSeed;
using Infrastructure.DependenciesInjection;
using Infrastructure.Services;
using Infrastructure.Services.EmailService;
using MEDSYstemITI.Hubs;
using MEDSYstemITI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentValidation;
using FluentValidation.AspNetCore;
using Application.Validators.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;



using Serilog;
using Serilog.Events;

namespace MEDSYstemITI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // =========================================================
            // Serilog Configuration
            // =========================================================


            var seqServerUrl =
                builder.Configuration["Serilog:SeqServerUrl"]
                ?? builder.Configuration["Seq:ServerUrl"]
                ?? "http://localhost:5341";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()

                // Reduce noisy framework logs
                .MinimumLevel.Override(
                    "Microsoft",
                    LogEventLevel.Warning)

                .MinimumLevel.Override(
                    "Microsoft.AspNetCore",
                    LogEventLevel.Warning)

                .MinimumLevel.Override(
                    "Microsoft.EntityFrameworkCore",
                    LogEventLevel.Warning)

                // Add useful context to every log
                .Enrich.FromLogContext()
                .Enrich.WithProperty(
                    "Application",
                    "MEDSystem")

                // Console logs
                .WriteTo.Console()

                // Seq logs
                .WriteTo.Seq(seqServerUrl)

                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.EnableServiceLogging();

            // =========================================================
            // Controllers
            // =========================================================

            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var traceId =
                            context.HttpContext.TraceIdentifier
                            ?? Guid.NewGuid().ToString();

                        var errors = context.ModelState
                            .Where(kvp =>
                                kvp.Value.Errors.Count > 0)
                            .SelectMany(kvp =>
                                kvp.Value.Errors.Select(e =>
                                    new Application.Common.Models.ErrorDetail
                                    {
                                        Field = kvp.Key,
                                        Message = e.ErrorMessage
                                    }))
                            .ToList();

                        var response =
                            new Application.Common.Models.ErrorResponse
                            {
                                StatusCode = 400,
                                Message = "Validation failed.",
                                ErrorCode = "VALIDATION_ERROR",
                                TraceId = traceId,
                                Errors = errors
                            };

                        return new BadRequestObjectResult(response);
                    };
                });

            // Configure FluentValidation auto-validation on the service collection (extension expects IServiceCollection)
            builder.Services.AddFluentValidationAutoValidation(options =>
            {
                options.DisableDataAnnotationsValidation = true;
            });

            builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestDtoValidator>();

            // =========================================================
            // OpenAPI / Swagger
            // =========================================================

            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API",
                    Version = "v1"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter JWT token only (without 'Bearer ').",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            });




            // AI (MedGemma) client used for PatientResult summarization/report generation and the RAG chatbot.
            builder.Services.AddMedGemmaAI(builder.Configuration);

            // MCP server exposing the AI tools in Application/MCPTools over HTTP at /mcp,
            // so any MCP-compatible client (Claude Desktop, Claude Code, an internal agent, ...)
            // can call GeneratePatientResultAIReport / GetPatientFullAIReport / AskPatientRagChatbot directly.
            builder.Services
                .AddMcpServer()
                .WithHttpTransport(options =>
                {
                    // Recommended when the server doesn't need server-to-client requests
                    // (sampling/elicitation) - simpler to scale horizontally behind a load balancer.
                    options.Stateless = true;
                })
                .WithToolsFromAssembly(typeof(PatientAIMcpTools).Assembly);

            // =========================================================
            // CORS
            // =========================================================

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DefaultCorsPolicy", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            // =========================================================
            // Application & Infrastructure
            // =========================================================

            builder.Services.AddinfrastructreServices(
                builder.Configuration,
                builder.Environment);

            builder.Services.AddApplicationServices();

            // =========================================================
            // SignalR
            // =========================================================

            builder.Services.AddSignalR();

            // =========================================================
            // File Storage
            // =========================================================

            builder.Services.AddScoped<
                IFileStorageService,
                FileStorageService>();

            // =========================================================
            // Email
            // =========================================================

            var emailConfig =
                builder.Configuration
                    .GetSection("EmailConfiguration")
                    .Get<EmailConfiguration>();

            builder.Services.AddSingleton(emailConfig!);


            builder.Services.AddScoped<
                IEmailSender,
                EmailSender>();

            // =========================================================
            // Build Application
            // =========================================================

            var app = builder.Build();

            // =========================================================
            // HTTP Pipeline
            // =========================================================

            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapOpenApi();
            //}

            app.UseStaticFiles();

            // Exception handling should be early
            app.UseGlobalExceptionHandling();

            app.UseHttpsRedirection();

            app.UseCors("DefaultCorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<NotificationHub>(
                "/notificationHub");
            app.MapMcp("/mcp");

            // =========================================================
            // Database Seeding
            // =========================================================

            // Run seeding except when running under the dedicated "Testing" environment.
            // Tests set the environment to "Testing" to prevent production seeding logic
            // (which runs SQL Server-specific commands) from executing against the test DB.
            if (!app.Environment.IsEnvironment("Testing"))
            {
                using (var scope = app.Services.CreateScope())
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                    try
                    {
                        logger.LogInformation("Starting database seeding");

                        await DbInitializer.SeedAsync(scope.ServiceProvider);

                        logger.LogInformation("Database seeding completed successfully");
                    }
                    catch (Exception ex)
                    {
                        logger.LogCritical(ex, "Database seeding failed");
                        throw;
                    }
                }
            }

            // =========================================================
            // Start Application
            // =========================================================

            try
            {
                Log.Information(
                    "MEDSystem application is starting");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(
                    ex,
                    "MEDSystem application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }


}
