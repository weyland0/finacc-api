using FluentValidation;
using FinaccApi.Modules.Counterparties.Domain;

namespace FinaccApi.Modules.Counterparties.Application.UpdateCounterparty;

/// <summary>
/// Валидатор команды обновления контрагента.
/// </summary>
public sealed class UpdateCounterpartyCommandValidator : AbstractValidator<UpdateCounterpartyCommand>
{
	/// <summary>
	/// Инициализирует правила проверки данных контрагента.
	/// </summary>
	public UpdateCounterpartyCommandValidator()
	{
		RuleFor(x => x.CounterpartyId).NotEmpty();

		RuleFor(x => x.Name)
			.NotEmpty()
			.MaximumLength(CounterpartyConstraints.MaxNameLength);

		RuleFor(x => x.Type).IsInEnum();

		RuleFor(x => x.Category)
			.MaximumLength(CounterpartyConstraints.MaxCategoryLength);

		RuleFor(x => x.Phone)
			.MaximumLength(CounterpartyConstraints.MaxPhoneLength);

		RuleFor(x => x.Email)
			.MaximumLength(CounterpartyConstraints.MaxEmailLength)
			.EmailAddress();
	}
}
