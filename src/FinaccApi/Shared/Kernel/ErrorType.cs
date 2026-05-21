namespace FinaccApi.Shared.Kernel;

/// <summary>
/// Типы ошибок в системе.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Ошибка по умолчанию. 
    /// </summary>
    Failure = 0,

    /// <summary>
    /// Ошибка валидации. Код ошибки в HTTP: 400
    /// </summary>
    Validation = 1,

    /// <summary>
    /// Запрошенный ресурс не найден. Код ошибки в HTTP: 404
    /// </summary>
    NotFound = 2,

    /// <summary>
    /// Конфликт ресурса. Код ошибки в HTTP: 409
    /// </summary>
    Conflict = 3,

    /// <summary>
    /// Ошибка авторизации. Код ошибки в HTTP: 401
    /// </summary>
    Unauthorized = 4,

    /// <summary>
    /// Ошибка запрета доступа. Код ошибки в HTTP: 403
    /// </summary>
    Forbidden = 5
}
