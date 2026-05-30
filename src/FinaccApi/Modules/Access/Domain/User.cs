using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Access.Domain;

/// <summary>
/// Представляет учетную запись пользователя системы.
/// </summary>
public class User : Entity
{
    /// <summary>
    /// Электронная почта пользователя, используемая для входа в систему.
    /// </summary>
    public string Email { get; private set; } = null!;

    /// <summary>
    /// Контактный телефон пользователя.
    /// </summary>
    public string Phone { get; set; } = null!;

    /// <summary>
    /// Хеш пароля пользователя.
    /// </summary>
    public string PasswordHash { get; private set; } = null!;

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Фамилия пользователя.
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Код текущего статуса пользователя.
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// Создает активного пользователя с указанными учетными данными.
    /// </summary>
    /// <param name="email">Электронная почта пользователя.</param>
    /// <param name="phone">Контактный телефон пользователя.</param>
    /// <param name="passwordHash">Хеш пароля пользователя.</param>
    /// <param name="firstName">Имя пользователя.</param>
    /// <param name="lastName">Фамилия пользователя.</param>
    public User(
        string email,
        string phone,
        string passwordHash,
        string firstName,
        string lastName)
    {
        Email = email;
        Phone = phone;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        Status = UserStatus.Active;
    }
}
