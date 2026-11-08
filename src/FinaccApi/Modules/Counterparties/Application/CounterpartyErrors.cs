using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Counterparties.Application;

/// <summary>
/// Ошибки модуля контрагентов.
/// </summary>
public static class CounterpartyErrors
{
	/// <summary>
	/// Контрагент не найден в текущей организации.
	/// </summary>
	public static readonly Error NotFound = Error.NotFound(
		"Counterparty.NotFound", "The requested counterparty does not exist.");

	/// <summary>
	/// У пользователя отсутствует активное членство в организации.
	/// </summary>
	public static readonly Error OrganizationNotFound = Error.NotFound(
		"Counterparty.OrganizationNotFound", "You are not a member of any organization.");

	/// <summary>
	/// У пользователя отсутствует необходимое разрешение.
	/// </summary>
	public static readonly Error Forbidden = Error.Forbidden(
		"Counterparty.Forbidden", "You do not have permission to perform this operation.");

	/// <summary>
	/// Архивированного контрагента нельзя изменить.
	/// </summary>
	public static readonly Error Archived = Error.Conflict(
		"Counterparty.Archived", "Archived counterparties cannot be updated.");

	/// <summary>
	/// Создает ошибку валидации входных данных.
	/// </summary>
	/// <param name="message">Описание ошибок валидации.</param>
	/// <returns>Ошибка валидации команды или запроса.</returns>
	public static Error BuildValidationError(string message) => Error.Validation(
		"Counterparty.Validation", message);
}
