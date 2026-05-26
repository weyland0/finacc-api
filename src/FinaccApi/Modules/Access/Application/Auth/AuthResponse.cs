namespace FinaccApi.Modules.Access.Application.Auth;

/// <summary>
/// Модель данных, возвращаемая при успешной аутентификации пользователя.
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Токен доступа для аутентифицированного пользователя.
    /// </summary>
    public string AccessToken { get; set; } = null!;

    /// <summary>
    /// Информация о текущем пользователе.
    /// </summary>
    public CurrentUserResponse CurrentUser { get; set; } = null!;
}
