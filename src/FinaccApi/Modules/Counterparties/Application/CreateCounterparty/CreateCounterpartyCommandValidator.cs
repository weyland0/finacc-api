using FluentValidation;
using FinaccApi.Modules.Counterparties.Domain;

namespace FinaccApi.Modules.Counterparties.Application.CreateCounterparty;

/// <summary>
/// Валидатор команды создания контрагента.
/// </summary>
public sealed class CreateCounterpartyCommandValidator : AbstractValidator<CreateCounterpartyCommand>
{
	/// <summary>
	/// Инициализирует правила проверки данных контрагента.
	/// </summary>
	public CreateCounterpartyCommandValidator()
	{
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
