using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Access.Domain;

/// <summary>
/// Модель пользователя.
/// </summary>
public class User : Entity
{
    /// <summary>
    /// Электронная почта пользователя.
    /// </summary>
    public string Email { get; private set; } = null!;

    /// <summary>
    /// Контактный телефон пользователя.
    /// </summary>
    public string Phone { get; set; } = null!;

    /// <summary>
    /// Хэш пароля пользователя.
    /// </summary>
    public string PasswordHash { get; private set; } = null!;

    /// <summary>
    /// Реальное имя пользователя.
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Фамилия пользователя.
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Статус пользователя. 
    /// (Active / Inactive)
    /// </summary>
    public string Status { get; set; } = null!;
}
