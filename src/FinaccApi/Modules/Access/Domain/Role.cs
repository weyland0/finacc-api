using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Access.Domain;

/// <summary>
/// Представляет роль пользователя в организации.
/// </summary>
public class Role : Entity
{
    /// <summary>
    /// Отображаемое название роли.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Системный код роли.
    /// </summary>
    public string Code { get; private set; } = null!;

    /// <summary>
    /// Внутренний список связей роли с разрешениями.
    /// </summary>
    private readonly List<RolePermission> _rolePermissions = new();

    /// <summary>
    /// Разрешения, назначенные роли через связи роль-разрешение.
    /// </summary>
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    /// <summary>
    /// Создает экземпляр роли для Entity Framework.
    /// </summary>
    private Role()
    {
    }

    /// <summary>
    /// Создает роль с указанным системным кодом и названием.
    /// </summary>
    /// <param name="code">Системный код роли.</param>
    /// <param name="name">Название роли.</param>
    public Role(string code, string name)
    {
        Code = code;
        Name = name;
    }
}
