using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Categories.Application;

/// <summary>
/// 
/// </summary>
public static class CategoryErrors
{
    /// <summary>
    /// Ошибка обращения к несуществующему пользователю
    /// или не активному пользователю.
    /// </summary>
    public static readonly Error CategoryNotFound = new(
        "Category.NotFound",
        "The requested category does not exist.",
        ErrorType.NotFound);

    public static readonly Error ParentCategoryNotFound = new(
        "Category.ParentCategoryNotFound",
        "The specified parent category does not exist.",
        ErrorType.NotFound);

    public static readonly Error Forbidden = new(
        "Category.Forbidden",
        "You do not have permission to perform this operation.",
        ErrorType.Forbidden);

    public static readonly Error OrganizationNotFound = new(
        "Category.OrganizationNotFound",
        "You are not a member of any organization.",
        ErrorType.NotFound);

    public static readonly Error ParentCategoryTypeConflict = new(
        "Category.ParentCategoryTypeConflict",
        "The specified category type is incompatible with the parent category type.",
        ErrorType.Conflict);
}
