using FinaccApi.Modules.Categories.Domain;

namespace FinaccApi.Modules.Categories.Application;

/// <summary>
/// Ответ получения конкретной категории.
/// </summary>
/// <param name="Id"> Идентификатор. </param>
/// <param name="Name"> Имя. </param>
/// <param name="CategoryType"> Тип. </param>
/// <param name="ActivityType"> Тип активности. </param>
/// <param name="Description"> Описание. </param>
/// <param name="OrganizationId"> Идентификатор организации. </param>
/// <param name="ParentCategoryId"> Идентификатор родительской категории. </param>
public record CategoryDetailsResponse(
	int Id,
	string Name,
	CategoryType CategoryType,
	ActivityType ActivityType,
	string? Description,
	int? OrganizationId,
	int? ParentCategoryId)
{
	/// <summary>
	/// Создает ответ получения конкретной категории из <paramref name="category"/>.
	/// </summary>
	/// <param name="category"> Категория. </param>
	/// <returns> Ответ получения конкретной категории. </returns>
	public static CategoryDetailsResponse CreateFromEntity(Category category)
	{
		ArgumentNullException.ThrowIfNull(category);

		return new CategoryDetailsResponse
		(
			category.Id,
			category.Name,
			category.CategoryType,
			category.ActivityType,
			category.Description,
			category.OrganizationId,
			category.ParentId
		);
	}
}
