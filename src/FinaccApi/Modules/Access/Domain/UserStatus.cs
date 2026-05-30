namespace FinaccApi.Modules.Access.Domain;

/// <summary>
/// Содержит системные коды статусов пользователя.
/// </summary>
public static class UserStatus
{
    /// <summary>
    /// Активный пользователь, которому разрешен вход в систему.
    /// </summary>
    public static string Active { get => "active"; }

    /// <summary>
    /// Неактивный пользователь, которому вход в систему ограничен.
    /// </summary>
    public static string Inactive { get => "inactive"; }
}
