using FinaccApi.Modules.Counterparties.Domain;

namespace FinaccApi.Modules.Counterparties.Application.UpdateCounterparty;

/// <summary>
/// Команда обновления контрагента.
/// </summary>
/// <param name="CounterpartyId">Идентификатор контрагента.</param>
/// <param name="Name">Название.</param>
/// <param name="Type">Тип контрагента.</param>
/// <param name="Category">Бизнес-классификация.</param>
/// <param name="Phone">Номер телефона.</param>
/// <param name="Email">Адрес электронной почты.</param>
public sealed record UpdateCounterpartyCommand(
	Guid CounterpartyId,
	string Name,
	CounterpartyType? Type,
	string? Category,
	string? Phone,
	string? Email);
