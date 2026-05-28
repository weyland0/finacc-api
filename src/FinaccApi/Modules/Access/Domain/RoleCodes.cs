namespace FinaccApi.Modules.Access.Domain;

/// <summary>
/// Класс, содержащий константы с кодами ролей доступа в системе.
/// </summary>
public static class RoleCodes
{
    /// <summary>
    /// Роль владельца организации.
    /// </summary>
    public const string Owner = "owner";

    /// <summary>
    /// Роль администратора организации.
    /// </summary>
    public const string Admin = "admin";

    /// <summary>
    /// Роль бухгалтера организации.
    /// </summary>
    public const string Accountant = "accountant";

    /// <summary>
    /// Роль зрителя организации.
    /// </summary>
    public const string Viewer = "viewer";
}
