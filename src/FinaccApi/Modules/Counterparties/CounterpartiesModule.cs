using FinaccApi.Modules.Counterparties.Application.ArchiveCounterparty;
using FinaccApi.Modules.Counterparties.Application.CreateCounterparty;
using FinaccApi.Modules.Counterparties.Application.GetCounterparties;
using FinaccApi.Modules.Counterparties.Application.GetCounterpartyById;
using FinaccApi.Modules.Counterparties.Application.UpdateCounterparty;
using FluentValidation;

namespace FinaccApi.Modules.Counterparties;

/// <summary>
/// Модуль контрагентов.
/// </summary>
public static class CounterpartiesModule
{
	/// <summary>
	/// Добавляет модуль контрагентов.
	/// </summary>
	/// <param name="services">Коллекция сервисов приложения.</param>
	/// <param name="configuration">Конфигурация приложения.</param>
	/// <returns>Коллекция сервисов приложения.</returns>
	public static IServiceCollection AddCounterpartiesModule(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		ArgumentNullException.ThrowIfNull(services);
		ArgumentNullException.ThrowIfNull(configuration);

		services.AddScoped<IValidator<CreateCounterpartyCommand>, CreateCounterpartyCommandValidator>();
		services.AddScoped<IValidator<UpdateCounterpartyCommand>, UpdateCounterpartyCommandValidator>();
		services.AddScoped<IValidator<ArchiveCounterpartyCommand>, ArchiveCounterpartyCommandValidator>();
		services.AddScoped<CreateCounterpartyHandler>();
		services.AddScoped<GetCounterpartiesHandler>();
		services.AddScoped<GetCounterpartyByIdHandler>();
		services.AddScoped<UpdateCounterpartyHandler>();
		services.AddScoped<ArchiveCounterpartyHandler>();

		return services;
	}
}
