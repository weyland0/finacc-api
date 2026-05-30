using FinaccApi.Modules.Access.Infrastructure;
using FinaccApi.Modules.Access.Application.Auth.Register;
using FinaccApi.Modules.Access.Application.Auth.Login;
using FinaccApi.Modules.Access.Application.Onboarding;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Shared.Application.Auth;

namespace FinaccApi.Modules.Access;

public static class AccessModule
{
    public static IServiceCollection AddAccessModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IPasswordHasher, PasswordHasherSha256>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentOrganization, CurrentOrganization>();
        services.AddScoped<IPermissionChecker, PermissionChecker>();

        services.AddScoped<RegisterHandler>();
        services.AddScoped<LoginHandler>();
        services.AddScoped<CreateOrganizationHandler>();

        return services;
    }
}
