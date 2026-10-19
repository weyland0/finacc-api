using FinaccApi.Modules.Categories.Application.CreateCategory;
using FinaccApi.Modules.Categories.Application.GetCategories;
using FinaccApi.Modules.Categories.Application.GetCategoryById;
using FinaccApi.Modules.Categories.Application.UpdateCategory;
using FluentValidation;

namespace FinaccApi.Modules.Categories;

/// <summary>
/// Модуль категорий.
/// </summary>
public static class CategoriesModule
{
	/// <summary>
	/// Добавляет сервисы и обработчики модуля категорий.
	/// </summary>
	/// <param name="services"> Коллекция сервисов приложения. </param>
	/// <param name="configuration"> Конфигурация приложения. </param>
	/// <returns> Коллекцию сервисов приложения. </returns>
	public static IServiceCollection AddCategoriesModule(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddScoped<
			IValidator<CreateCategoryCommand>,
			CreateCategoryCommandValidator>();

		services.AddScoped<
			IValidator<UpdateCategoryCommand>,
			UpdateCategoryCommandValidator>();

		services.AddScoped<CreateCategoryHandler>();
		services.AddScoped<GetCategoriesHandler>();
		services.AddScoped<GetCategoryByIdHandler>();
		services.AddScoped<UpdateCategoryHandler>();

		return services;
	}
}
