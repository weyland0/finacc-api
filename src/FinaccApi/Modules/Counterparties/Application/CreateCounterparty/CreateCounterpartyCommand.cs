using FinaccApi.Modules.Counterparties.Domain;

namespace FinaccApi.Modules.Counterparties.Application.CreateCounterparty;

/// <summary>
/// Команда создания контрагента.
/// </summary>
/// <param name="Name">Название или ФИО.</param>
/// <param name="Type">Тип контрагента.</param>
/// <param name="Category">Бизнес-классификация.</param>
/// <param name="Phone">Номер телефона.</param>
/// <param name="Email">Адрес электронной почты.</param>
public sealed record CreateCounterpartyCommand(
	string Name,
	CounterpartyType? Type,
	string? Category,
	string? Phone,
	string? Email);
