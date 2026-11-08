using FluentValidation;

namespace FinaccApi.Modules.Counterparties.Application.ArchiveCounterparty;

/// <summary>
/// Валидатор команды архивирования контрагента.
/// </summary>
public sealed class ArchiveCounterpartyCommandValidator : AbstractValidator<ArchiveCounterpartyCommand>
{
	/// <summary>
	/// Инициализирует правила проверки идентификатора контрагента.
	/// </summary>
	public ArchiveCounterpartyCommandValidator()
	{
		RuleFor(x => x.CounterpartyId).GreaterThan(0);
	}
}
