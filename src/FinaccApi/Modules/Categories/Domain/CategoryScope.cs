namespace FinaccApi.Modules.Categories.Domain;

public enum CategoryScope
/// <summary>
/// Область поиска категории.
/// </summary>
{
	/// <summary>
	/// Все доступные.
	/// </summary>
	AllAvailable = 1,

	/// <summary>
	/// Только организационные.
	/// </summary>
	OnlyOrganization = 2,

	/// <summary>
	/// Только глобальные.
	/// </summary>
	OnlyGlobal = 3
}
