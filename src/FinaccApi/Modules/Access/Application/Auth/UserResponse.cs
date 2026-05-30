namespace FinaccApi.Modules.Access.Application.Auth;

/// <summary>
/// Данные текущего пользователя, возвращаемые клиенту после аутентификации.
/// </summary>
public class UserResponse
{
    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public int Id { get; set; }

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
}
