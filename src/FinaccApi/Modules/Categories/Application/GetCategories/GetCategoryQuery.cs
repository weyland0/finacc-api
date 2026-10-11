using FinaccApi.Modules.Categories.Domain;

namespace FinaccApi.Modules.Categories.Application.GetCategories;

/// <summary>
/// Запрос получения категорий.
/// </summary>
/// <param name="CategoryType"> Тип. </param>
/// <param name="ActivityType"> Тип активности. </param>
/// <param name="Scope"> Область поиска. </param>
/// <param name="Search"> Фильтр. </param>
/// <param name="AsTree"> Признак формирования коллекции как дерево. </param>
public record GetCategoriesQuery(
	CategoryType? CategoryType = null,
	ActivityType? ActivityType = null,
	CategorySearchScope Scope = CategorySearchScope.AllAvailable,
	string? Search = null,
	bool AsTree = true
);

