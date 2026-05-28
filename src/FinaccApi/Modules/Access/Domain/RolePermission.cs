namespace FinaccApi.Modules.Access.Domain;

/// <summary>
/// Модель связи между ролью и правом доступа.
/// Показывает какие права доступа есть у роли.
/// </summary>
public class RolePermission
{
    /// <summary>
    /// Идентификатор роли.
    /// </summary>
    public int RoleId { get; private set; }
    public Role Role { get; private set; } = null!;

    /// <summary>
    /// Идентификатор права доступа.
    /// </summary>
    public int PermissionId { get; private set; }
    public Permission Permission { get; private set; } = null!;
}
