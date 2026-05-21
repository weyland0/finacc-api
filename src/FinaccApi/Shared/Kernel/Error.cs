namespace FinaccApi.Shared.Kernel;

/// <summary>
/// Ошибка.
/// </summary>
public sealed record Error(
    /// <summary>
    /// Код ошибки. Например: "Accounts.NotFound"
    /// </summary>
    string Code,

    /// <summary>
    /// Описание ошибки.
    /// </summary>
    string Message,

    /// <summary>
    /// Тип ошибки.
    /// </summary>
    ErrorType Type = ErrorType.Failure)
{
    /// <summary>
    /// Пустая ошибка.
    /// </summary>
    public static readonly Error None = new(
        Code: string.Empty,
        Message: string.Empty,
        Type: ErrorType.Failure);

    /// <summary>
    /// Создает ошибку валидации.
    /// </summary>
    /// <param name="code">Код ошибки.</param>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <returns>Ошибка с типом ErrorType.Validation</returns>
    public static Error Validation(string code, string message)
        => new(code, message, ErrorType.Validation);

    /// <summary>
    /// Создает ошибку запрашеваемый ресурс не найден.
    /// </summary>
    /// <param name="code">Код ошибки.</param>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <returns>Ошибка с типом ErrorType.NotFound</returns>
    public static Error NotFound(string code, string message)
        => new(code, message, ErrorType.NotFound);

    /// <summary>
    /// Создает ошибку конфликта ресурса.
    /// </summary>
    /// <param name="code">Код ошибки.</param>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <returns>Ошибка с типом ErrorType.Conflict</returns>
    public static Error Conflict(string code, string message)
        => new(code, message, ErrorType.Conflict);

    /// <summary>
    /// Создает ошибку авторизации.
    /// </summary>
    /// <param name="code">Код ошибки.</param>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <returns>Ошибка с типом ErrorType.Unauthorized</returns>
    public static Error Unauthorized(string code, string message)
        => new(code, message, ErrorType.Unauthorized);

    /// <summary>
    /// Создает ошибку запрета доступа.
    /// </summary>
    /// <param name="code">Код ошибки.</param>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <returns>Ошибка с типом ErrorType.Forbidden</returns>
    public static Error Forbidden(string code, string message)
        => new(code, message, ErrorType.Forbidden);

    /// <summary>
    /// Создает ошибку по умолчанию.
    /// </summary>
    /// <param name="code">Код ошибки.</param>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <returns>Ошибка с типом ErrorType.Failure</returns>
    public static Error Failure(string code, string message)
        => new(code, message, ErrorType.Failure);
}
