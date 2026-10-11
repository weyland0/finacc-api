namespace FinaccApi.Modules.Categories.Domain;

/// <summary>
/// Область поиска категории.
/// </summary>
public enum CategorySearchScope
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
