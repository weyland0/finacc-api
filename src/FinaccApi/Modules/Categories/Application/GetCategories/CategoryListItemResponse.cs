using FinaccApi.Modules.Categories.Domain;

namespace FinaccApi.Modules.Categories.Application;

/// <summary>
/// Элемент ответа получения категорий.
/// </summary>
/// <param name="Id"> Идентификатор категории. </param>
/// <param name="Name"> Имя. </param>
/// <param name="CategoryType"> Тип. </param>
/// <param name="ActivityType"> Тип активности. </param>
/// <param name="Description"> Описание. </param>
/// <param name="OrganizationId"> Идентификатор организации. </param>
/// <param name="ParentCategoryId"> Идентификатор родительской категории. </param>
/// <param name="Children"> Дочерние категории. </param>
public record CategoryListItemResponse(
	Guid Id,
	string Name,
	CategoryType CategoryType,
	ActivityType ActivityType,
	string? Description,
	Guid? OrganizationId,
	Guid? ParentCategoryId,
	IReadOnlyList<CategoryListItemResponse> Children
);
