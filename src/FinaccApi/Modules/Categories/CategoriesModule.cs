using FinaccApi.Modules.Categories.Application.CreateCategory;

namespace FinaccApi.Modules.Categories;

/// <summary>
/// Регистрирует сервисы модуля доступа в контейнере зависимостей.
/// </summary>
public static class CategoriesModule
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
        services.AddScoped<CreateCategoryHandler>();
        services.AddScoped<CreateCategoryCommandValidator>();

        return services;
    }
}
