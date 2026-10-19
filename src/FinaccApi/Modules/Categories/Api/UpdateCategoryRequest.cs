namespace FinaccApi.Modules.Categories.Api;

/// <summary>
/// Запрос обновления категории.
/// </summary>
/// <param name="Name"> Название категории. </param>
/// <param name="Description"> Описание категории. </param>
public record UpdateCategoryRequest(
	string Name,
	string? Description
);
