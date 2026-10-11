using FinaccApi.Modules.Categories.Domain;

namespace FinaccApi.Modules.Categories.Application.CreateCategory;

/// <summary>
/// Команда создания категории.
/// </summary>
/// <param name="Name"> Имя. </param>
/// <param name="CategoryType"> Тип. </param>
/// <param name="ActivityType"> Тип активности. </param>
/// <param name="Description"> Описание. </param>
/// <param name="ParentCategoryId"> Идентификатор родительской категории. </param>
public record CreateCategoryCommand(
	string Name,
	CategoryType CategoryType,
	ActivityType ActivityType,
	string? Description,
	int? ParentCategoryId
);
