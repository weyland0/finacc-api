using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Categories.Application;

/// <summary>
/// Ошибки модуля категории.
/// </summary>
public static class CategoryErrors
{
    /// <summary>
    /// Ошибка обращения к несуществующей категории.
    /// </summary>
    public static readonly Error CategoryNotFound = new(
        "Category.NotFound",
        "The requested category does not exist.",
        ErrorType.NotFound);

	/// <summary>
	/// Ошибка родительская категория не найдена.
	/// </summary>
    public static readonly Error ParentCategoryNotFound = new(
        "Category.ParentCategoryNotFound",
        "The specified parent category does not exist.",
        ErrorType.NotFound);

	/// <summary>
	/// Ошибка отсутствия доступа на выполнение операции модуля.
	/// </summary>
    public static readonly Error Forbidden = new(
        "Category.Forbidden",
        "You do not have permission to perform this operation.",
        ErrorType.Forbidden);

	/// <summary>
	/// Ошибка организация не найдена.
	/// </summary>
    public static readonly Error OrganizationNotFound = new(
        "Category.OrganizationNotFound",
        "You are not a member of any organization.",
        ErrorType.NotFound);

	/// <summary>
	/// Ошибка совместимости категории с родительской категорией.
	/// </summary>
    public static readonly Error ParentCategoryTypeConflict = new(
        "Category.ParentCategoryTypeConflict",
        "The specified category type is incompatible with the parent category type.",
        ErrorType.Conflict);

	/// <summary>
	/// Создает ошибку валидации входных данных.
	/// </summary>
	/// <param name="message"> Сообщение об ошибке валидации. </param>
	/// <returns></returns>
	public static Error BuildValidationError(string message) => new(
        "Category.Validation",
        message,
        ErrorType.Validation);
}
