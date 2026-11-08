using FinaccApi.Modules.Counterparties.Domain;

namespace FinaccApi.Modules.Counterparties.Api;

/// <summary>
/// HTTP-запрос полной замены данных контрагента.
/// </summary>
/// <param name="Name">Название или ФИО.</param>
/// <param name="Type">Тип контрагента или null для очистки.</param>
/// <param name="Category">Бизнес-классификация или null для очистки.</param>
/// <param name="Phone">Номер телефона или null для очистки.</param>
/// <param name="Email">Адрес электронной почты или null для очистки.</param>
public sealed record UpdateCounterpartyRequest(
	string Name,
	CounterpartyType? Type,
	string? Category,
	string? Phone,
	string? Email);
