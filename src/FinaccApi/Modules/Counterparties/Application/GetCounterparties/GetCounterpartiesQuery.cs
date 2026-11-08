using FinaccApi.Modules.Counterparties.Domain;

namespace FinaccApi.Modules.Counterparties.Application.GetCounterparties;

/// <summary>
/// Запрос списка контрагентов.
/// </summary>
/// <param name="Search">Поисковый запрос.</param>
/// <param name="Type">Фильтр по типу контрагента.</param>
/// <param name="IncludeArchived">Включать архивированных контрагентов.</param>
public sealed record GetCounterpartiesQuery(
	string? Search = null,
	CounterpartyType? Type = null,
	bool IncludeArchived = false);
