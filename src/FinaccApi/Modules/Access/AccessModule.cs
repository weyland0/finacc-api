using FinaccApi.Shared.Infrastructure;
using FinaccApi.Modules.Access.Application.Auth.Register;
using FinaccApi.Modules.Access.Infrastructure;

namespace FinaccApi.Modules.Access;

public static class AccessModule
{
    public static IServiceCollection AddAccessModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IPasswordHasher, PasswordHasherSha256>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddScoped<RegisterHandler>();

        return services;
    }
}