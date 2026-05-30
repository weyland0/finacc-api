using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Access.Application.Auth;

/// <summary>
/// Содержит ошибки сценариев аутентификации и регистрации пользователей.
/// </summary>
public static class AuthErrors
{
    /// <summary>
    /// Ошибка регистрации, если пользователь с таким email уже существует.
    /// </summary>
    public static readonly Error EmailAlreadyExists = new(
        "Auth.EmailAlreadyExists",
        "User with this email already exists.",
        ErrorType.Conflict);

    /// <summary>
    /// Ошибка регистрации, если пользователь с таким телефоном уже существует.
    /// </summary>
    public static readonly Error PhoneAlreadyExists = new(
        "Auth.PhoneAlreadyExists",
        "User with this phone already exists.",
        ErrorType.Conflict);

    /// <summary>
    /// Ошибка входа при неверном email или пароле.
    /// </summary>
    public static readonly Error InvalidCredentials = new(
        "Auth.InvalidCredentials",
        "Invalid email or password.",
        ErrorType.Validation);

    /// <summary>
    /// Ошибка обращения к несуществующему пользователю.
    /// </summary>
    public static readonly Error UserNotFound = new(
        "Auth.UserNotFound",
        "User was not found.",
        ErrorType.NotFound);

    /// <summary>
    /// Ошибка обращения без аутентификации.
    /// </summary>
    public static readonly Error Unauthorized = new(
        "Auth.Unauthorized",
        "User is not authenticated.",
        ErrorType.Unauthorized);
}
