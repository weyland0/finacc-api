using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Access.Domain;

/// <summary>
/// Модель роли.
/// Показывает кем является пользователь в организации.
/// </summary>
public class Role : Entity
{
    /// <summary>
    /// Название роли.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Код роли - системное обозначение роли.
    /// </summary>
    public string Code { get; private set; } = null!;

    /// <summary>
    /// Связанные права доступа роли.
    /// </summary>
    private readonly List<RolePermission> _rolePermissions = new();
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    /// <summary>
    /// Конструктор роли.
    /// Используется только для Entity Framework.
    /// </summary>
    private Role()
    {
    }

    /// <summary>
    /// Конструктор роли.
    /// </summary>
    /// <param name="code">Системный код роли.</param>
    /// <param name="name">Название роли.</param>
    public Role(string code, string name)
    {
        Code = code;
        Name = name;
    }
}
