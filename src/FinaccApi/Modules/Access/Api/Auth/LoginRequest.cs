namespace FinaccApi.Modules.Access.Api.Auth;


/// <summary>
/// HTTP-запрос на вход пользователя в систему.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Электронная почта пользователя.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    public string Password { get; set; } = null!;
}
