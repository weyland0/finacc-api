namespace FinaccApi.Modules.Counterparties.Domain;

/// <summary>
/// Тип контрагента.
/// </summary>
public enum CounterpartyType
{
	/// <summary>
	/// Физическое лицо.
	/// </summary>
	Individual = 1,

	/// <summary>
	/// Юридическое лицо.
	/// </summary>
	LegalEntity = 2
}
