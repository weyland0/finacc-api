using FinaccApi.Modules.Access.Infrastructure;
using FinaccApi.Modules.Access.Application.Auth.Register;
using FinaccApi.Modules.Access.Application.Auth.Login;

namespace FinaccApi.Modules.Access;

public static class AccessModule
{
    public static IServiceCollection AddAccessModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IPasswordHasher, PasswordHasherSha256>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddScoped<RegisterHandler>();
        services.AddScoped<LoginHandler>();

        return services;
    }
}