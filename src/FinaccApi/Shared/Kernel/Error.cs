namespace FinaccApi.Shared.Kernel;

/// <summary>
/// Описывает ошибку приложения с кодом, сообщением и типом для HTTP-представления.
/// </summary>
public sealed record Error(
    /// <summary>
    /// Машиночитаемый код ошибки, например "Accounts.NotFound".
    /// </summary>
    string Code,

    /// <summary>
    /// Человекочитаемое сообщение об ошибке.
    /// </summary>
    string Message,

    /// <summary>
    /// Тип ошибки, определяющий HTTP-статус ответа.
    /// </summary>
    ErrorType Type = ErrorType.Failure)
{
    /// <summary>
    /// Пустая ошибка, используемая для успешных результатов.
    /// </summary>
    public static readonly Error None = new(
        Code: string.Empty,
        Message: string.Empty,
        Type: ErrorType.Failure);

    /// <summary>
    /// Создает ошибку валидации входных данных.
    /// </summary>
    /// <param name="code">Код ошибки.</param>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <returns>Ошибка с типом <see cref="ErrorType.Validation"/>.</returns>
    public static Error Validation(string code, string message)
        => new(code, message, ErrorType.Validation);

    /// <summary>
    /// Создает ошибку отсутствия запрошенного ресурса.
    /// </summary>
    /// <param name="code">Код ошибки.</param>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <returns>Ошибка с типом <see cref="ErrorType.NotFound"/>.</returns>
    public static Error NotFound(string code, string message)
        => new(code, message, ErrorType.NotFound);

    /// <summary>
    /// Создает ошибку конфликта состояния ресурса.
    /// </summary>
    /// <param name="code">Код ошибки.</param>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <returns>Ошибка с типом <see cref="ErrorType.Conflict"/>.</returns>
    public static Error Conflict(string code, string message)
        => new(code, message, ErrorType.Conflict);

    /// <summary>
    /// Создает ошибку отсутствия аутентификации.
    /// </summary>
    /// <param name="code">Код ошибки.</param>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <returns>Ошибка с типом <see cref="ErrorType.Unauthorized"/>.</returns>
    public static Error Unauthorized(string code, string message)
        => new(code, message, ErrorType.Unauthorized);

    /// <summary>
    /// Создает ошибку запрета доступа к ресурсу.
    /// </summary>
    /// <param name="code">Код ошибки.</param>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <returns>Ошибка с типом <see cref="ErrorType.Forbidden"/>.</returns>
    public static Error Forbidden(string code, string message)
        => new(code, message, ErrorType.Forbidden);

    /// <summary>
    /// Создает общую ошибку выполнения операции.
    /// </summary>
    /// <param name="code">Код ошибки.</param>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <returns>Ошибка с типом <see cref="ErrorType.Failure"/>.</returns>
    public static Error Failure(string code, string message)
        => new(code, message, ErrorType.Failure);
}
