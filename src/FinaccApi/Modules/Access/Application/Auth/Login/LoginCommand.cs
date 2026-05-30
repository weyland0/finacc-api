namespace FinaccApi.Modules.Access.Application.Auth.Login;


/// <summary>
/// Команда приложения для входа пользователя в систему.
/// </summary>
public class LoginCommand
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
