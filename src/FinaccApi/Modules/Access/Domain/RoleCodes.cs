namespace FinaccApi.Modules.Access.Domain;

/// <summary>
/// Содержит системные коды ролей доступа в организации.
/// </summary>
public static class RoleCodes
{
    /// <summary>
    /// Владелец организации с максимальными правами управления.
    /// </summary>
    public const string Owner = "owner";

    /// <summary>
    /// Администратор организации с правами управления пользователями и настройками.
    /// </summary>
    public const string Admin = "admin";

    /// <summary>
    /// Бухгалтер организации, работающий с финансовыми данными.
    /// </summary>
    public const string Accountant = "accountant";

    /// <summary>
    /// Пользователь с правами просмотра без изменения данных.
    /// </summary>
    public const string Viewer = "viewer";
}
