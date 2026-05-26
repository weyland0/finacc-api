namespace FinaccApi.Modules.Access.Application.Auth;

/// <summary>
/// Модель данных для ответа при запросе информации о текущем пользователе.
/// </summary>
public class CurrentUserResponse
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
}
