using FinaccApi.Modules.Counterparties.Domain;

namespace FinaccApi.Modules.Counterparties.Api;

/// <summary>
/// HTTP-запрос создания контрагента в текущей организации.
/// </summary>
/// <param name="Name">Название или ФИО.</param>
/// <param name="Type">Тип контрагента.</param>
/// <param name="Category">Бизнес-классификация.</param>
/// <param name="Phone">Номер телефона.</param>
/// <param name="Email">Адрес электронной почты.</param>
public sealed record CreateCounterpartyRequest(
	string Name,
	CounterpartyType? Type,
	string? Category,
	string? Phone,
	string? Email);
