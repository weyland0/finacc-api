using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Access.Application.Auth;

/// <summary>
/// Класс, содержащий определения ошибок, 
/// которые могут возникать в процессе аутентификации
/// и авторизации пользователей.
/// </summary>
public static class AuthErrors
{
    public static readonly Error EmailAlreadyExists = new(
        "Auth.EmailAlreadyExists",
        "User with this email already exists.",
        ErrorType.Conflict);

    public static readonly Error PhoneAlreadyExists = new(
        "Auth.PhoneAlreadyExists",
        "User with this phone already exists.",
        ErrorType.Conflict);

    public static readonly Error InvalidCredentials = new(
        "Auth.InvalidCredentials",
        "Invalid email or password.",
        ErrorType.Validation);

    public static readonly Error UserNotFound = new(
        "Auth.UserNotFound",
        "User was not found.",
        ErrorType.NotFound);

    public static readonly Error Unauthorized = new(
        "Auth.Unauthorized",
        "User is not authenticated.",
        ErrorType.Unauthorized);
}
