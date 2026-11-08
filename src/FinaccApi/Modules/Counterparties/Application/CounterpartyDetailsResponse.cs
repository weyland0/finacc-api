using FinaccApi.Modules.Counterparties.Domain;

namespace FinaccApi.Modules.Counterparties.Application;

/// <summary>
/// Ответ операции с контрагентом.
/// </summary>
/// <param name="Id">Идентификатор контрагента.</param>
/// <param name="OrganizationId">Идентификатор организации.</param>
/// <param name="Name">Название.</param>
/// <param name="Type">Тип контрагента.</param>
/// <param name="Category">Бизнес-классификация.</param>
/// <param name="Phone">Номер телефона.</param>
/// <param name="Email">Адрес электронной почты.</param>
/// <param name="IsArchived">Признак архивирования.</param>
public sealed record CounterpartyDetailsResponse(
	Guid Id,
	Guid OrganizationId,
	string Name,
	CounterpartyType? Type,
	string? Category,
	string? Phone,
	string? Email,
	bool IsArchived)
{
	/// <summary>
	/// Создает ответ из доменной сущности контрагента.
	/// </summary>
	/// <param name="counterparty">Контрагент.</param>
	/// <returns>Данные контрагента.</returns>
	public static CounterpartyDetailsResponse CreateFromEntity(Counterparty counterparty)
	{
		ArgumentNullException.ThrowIfNull(counterparty);

		return new CounterpartyDetailsResponse(
			counterparty.Id,
			counterparty.OrganizationId,
			counterparty.Name,
			counterparty.Type,
			counterparty.Category,
			counterparty.Phone,
			counterparty.Email,
			counterparty.IsArchived);
	}
}
