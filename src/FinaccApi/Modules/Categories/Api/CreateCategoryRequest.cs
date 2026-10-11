using FinaccApi.Modules.Categories.Domain;

namespace FinaccApi.Modules.Categories.Api;

/// <summary>
/// Запрос создания категории.
/// </summary>
/// <param name="Name"></param>
/// <param name="CategoryType"></param>
/// <param name="ActivityType"></param>
/// <param name="Description"></param>
/// <param name="ParentCategoryId"></param>
public record CreateCategoryRequest(
	string Name,
	CategoryType CategoryType,
	ActivityType ActivityType,
	string? Description,
	int? ParentCategoryId
);
