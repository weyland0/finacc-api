namespace FinaccApi.Shared.Application.Auth;

/// <summary>
/// Предоставляет данные пользователя, выполняющего текущий запрос.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Идентификатор текущего пользователя, если запрос аутентифицирован.
    /// </summary>
    int? UserId { get; }

    /// <summary>
    /// Электронная почта текущего пользователя, если запрос аутентифицирован.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Признак наличия аутентифицированного пользователя в текущем запросе.
    /// </summary>
    bool IsAuthenticated { get; }
}
