namespace FinaccApi.Modules.Categories.Application.GetCategoryById;

/// <summary>
/// Запрос получения категории по идентификатору.
/// </summary>
/// <param name="CategoryId"> Идентификатор категории. </param>
public record GetCategoryByIdQuery(int CategoryId);
