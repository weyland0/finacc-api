using FinaccApi.Modules.Organizations.Application.GetOrganization;
using FinaccApi.Modules.Organizations.Application.UpdateOrganization;

namespace FinaccApi.Modules.Organizations;

/// <summary>
/// Регистрирует сервисы модуля доступа в контейнере зависимостей.
/// </summary>
public static class OrganizationsModule
{
    /// <summary>
    /// Добавляет инфраструктурные сервисы и обработчики модуля доступа.
    /// </summary>
    /// <param name="services">Коллекция сервисов приложения.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Та же коллекция сервисов для дальнейшей настройки.</returns>
    public static IServiceCollection AddOrganizationsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<GetOrganizationHandler>();
        services.AddScoped<UpdateOrganizationHandler>();

        return services;
    }
}
