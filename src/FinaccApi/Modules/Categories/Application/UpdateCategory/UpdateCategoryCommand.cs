namespace FinaccApi.Modules.Categories.Application.UpdateCategory;

/// <summary>
/// Команда обновления категории.
/// </summary>
/// <param name="CategoryId"> Идентификатор. </param>
/// <param name="Name"> Название. </param>
/// <param name="Description"> Описание категории. </param>
public record UpdateCategoryCommand(
	int CategoryId,
	string Name,
	string? Description
);
