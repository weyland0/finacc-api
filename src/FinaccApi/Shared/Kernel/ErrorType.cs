namespace FinaccApi.Shared.Kernel;

/// <summary>
/// Типы ошибок, используемые для преобразования доменных ошибок в HTTP-ответы.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Общая ошибка выполнения операции. Обычно соответствует HTTP 500.
    /// </summary>
    Failure = 0,

    /// <summary>
    /// Ошибка валидации входных данных. Соответствует HTTP 400.
    /// </summary>
    Validation = 1,

    /// <summary>
    /// Запрошенный ресурс не найден. Соответствует HTTP 404.
    /// </summary>
    NotFound = 2,

    /// <summary>
    /// Конфликт состояния ресурса. Соответствует HTTP 409.
    /// </summary>
    Conflict = 3,

    /// <summary>
    /// Пользователь не аутентифицирован. Соответствует HTTP 401.
    /// </summary>
    Unauthorized = 4,

    /// <summary>
    /// Пользователь аутентифицирован, но не имеет доступа. Соответствует HTTP 403.
    /// </summary>
    Forbidden = 5
}
