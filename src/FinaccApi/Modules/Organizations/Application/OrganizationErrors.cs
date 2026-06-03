using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Organizations.Application;

/// <summary>
/// 
/// </summary>
public static class OrganizationErrors
{
    /// <summary>
    /// Ошибка обращения к несуществующему пользователю
    /// или не активному пользователю.
    /// </summary>
    public static readonly Error OrganizationNotFound = new(
        "Organization.NotFound",
        "You are not a member of any organization.",
        ErrorType.NotFound);

    public static readonly Error Forbidden = new(
        "Organization.Forbidden",
        "You do not have permission to perform this operation.",
        ErrorType.Forbidden);
}
