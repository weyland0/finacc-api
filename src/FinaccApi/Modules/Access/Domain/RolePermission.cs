namespace FinaccApi.Modules.Access.Domain;

/// <summary>
/// Представляет связь между ролью и разрешением, назначенным этой роли.
/// </summary>
public class RolePermission
{
    /// <summary>
    /// Идентификатор роли, которой назначено разрешение.
    /// </summary>
    public Guid RoleId { get; private set; }

    /// <summary>
    /// Роль, которой назначено разрешение.
    /// </summary>
    public Role Role { get; private set; } = null!;

    /// <summary>
    /// Идентификатор разрешения, назначенного роли.
    /// </summary>
    public Guid PermissionId { get; private set; }

    /// <summary>
    /// Разрешение, назначенное роли.
    /// </summary>
    public Permission Permission { get; private set; } = null!;
}
