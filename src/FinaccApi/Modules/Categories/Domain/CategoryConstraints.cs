using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Categories.Domain;

public class CategoryConstraints
{
	/// <summary>
	/// Имя таблицы.
	/// </summary>
	public const string TableName = "categories";

	/// <summary>
	/// Максимальная длина имени.
	/// </summary>
	public const int MaxNameLength = 200;

	/// <summary>
	/// Максимальная длина описания.
	/// </summary>
	public const int MaxDescriptionLength = 1000;

	/// <summary>
	/// Максимальная длина типа категории.
	/// </summary>
	public const int MaxCategoryTypeLength = 50;

	/// <summary>
	/// Максимальная длина типа активности.
	/// </summary>
	public const int MaxActivityTypeLength = 50;
}