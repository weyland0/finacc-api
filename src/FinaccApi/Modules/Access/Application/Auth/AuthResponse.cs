namespace FinaccApi.Modules.Access.Application.Auth;

/// <summary>
/// Данные, возвращаемые после успешной аутентификации пользователя.
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Access token для обращения к защищенным ресурсам API.
    /// </summary>
    public string AccessToken { get; set; } = null!;

    /// <summary>
    /// Профиль аутентифицированного пользователя.
    /// </summary>
    public CurrentUserResponse CurrentUser { get; set; } = null!;
}
