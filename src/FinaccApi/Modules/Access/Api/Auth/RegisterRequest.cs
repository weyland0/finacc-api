namespace FinaccApi.Modules.Access.Api.Auth;


/// <summary>
/// HTTP-запрос на регистрацию нового пользователя.
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Электронная почта пользователя.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Контактный телефон пользователя.
    /// </summary>
    public string Phone { get; set; } = null!;

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Фамилия пользователя.
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    public string Password { get; set; } = null!;
}
