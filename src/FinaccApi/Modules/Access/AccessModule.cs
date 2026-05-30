using FinaccApi.Modules.Access.Infrastructure;
using FinaccApi.Modules.Access.Application.Auth.Register;
using FinaccApi.Modules.Access.Application.Auth.Login;
using FinaccApi.Modules.Access.Application.Onboarding;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Shared.Application.Auth;

namespace FinaccApi.Modules.Access;

/// <summary>
/// Регистрирует сервисы модуля доступа в контейнере зависимостей.
/// </summary>
public static class AccessModule
{
    /// <summary>
    /// Добавляет инфраструктурные сервисы и обработчики модуля доступа.
    /// </summary>
    /// <param name="services">Коллекция сервисов приложения.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Та же коллекция сервисов для дальнейшей настройки.</returns>
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
