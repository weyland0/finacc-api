using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Access.Application.UserContext;

/// <summary>
/// 
/// </summary>
public static class UserContextErrors
{
    /// <summary>
    /// Ошибка обращения к несуществующему пользователю
    /// или не активному пользователю.
    /// </summary>
    public static readonly Error UserNotFoundOrInactive = new(
        "UserContext.UserNotFound",
        "User was not found or user is inactive right know.",
        ErrorType.NotFound);

    /// <summary>
    /// Ошибка обращения без аутентификации.
    /// </summary>
    public static readonly Error Unauthorized = new(
        "UserContext.Unauthorized",
        "User is not authenticated.",
        ErrorType.Unauthorized);
}
