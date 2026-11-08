namespace FinaccApi.Modules.Counterparties.Domain;

/// <summary>
/// Ограничения данных контрагента.
/// </summary>
public static class CounterpartyConstraints
{
	/// <summary>
	/// Имя таблицы контрагентов.
	/// </summary>
	public const string TableName = "counterparties";

	/// <summary>
	/// Максимальная длина названия или ФИО.
	/// </summary>
	public const int MaxNameLength = 200;

	/// <summary>
	/// Максимальная длина бизнес-классификации.
	/// </summary>
	public const int MaxCategoryLength = 100;

	/// <summary>
	/// Максимальная длина номера телефона.
	/// </summary>
	public const int MaxPhoneLength = 50;

	/// <summary>
	/// Максимальная длина адреса электронной почты.
	/// </summary>
	public const int MaxEmailLength = 200;

	/// <summary>
	/// Максимальная длина строкового представления типа.
	/// </summary>
	public const int MaxTypeLength = 50;
}
