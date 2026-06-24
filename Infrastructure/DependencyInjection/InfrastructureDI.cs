using Application.Interfaces.Services;
using Domain.Repositories;
using Infrastructure.Configurations;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class InfrastructureDI
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Core infrastructure services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IMemberRepo, MemberRepo>();

        // Register application service implementations that depend on infrastructure
        services.AddScoped<IAuthService, Application.Services.AuthService>();

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        return services;
    }
}
