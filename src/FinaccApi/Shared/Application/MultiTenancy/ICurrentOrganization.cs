namespace FinaccApi.Shared.Application.MultiTenancy;

/// <summary>
/// Интерфейс для получения информации о текущей организации, к которой принадлежит пользователь.
/// </summary>
public interface ICurrentOrganization
{
    /// <summary>
    /// Идентификатор организации, к которой принадлежит текущий пользователь.
    /// </summary>
    int? OrganizationId { get; }

    /// <summary>
    /// Идентификатор членства пользователя в организации, если применимо.
    /// </summary>
    int? MembershipId { get; }

    /// <summary>
    /// Идентификатор роли пользователя в организации, если применимо.
    /// </summary>
    int? RoleId { get; }

    /// <summary>
    /// Набор разрешений, связанных с ролью пользователя в организации.
    /// </summary>
    IReadOnlyList<string> Permissions { get; }

    /// <summary>
    /// Указывает, принадлежит ли текущий пользователь какой-либо организации.
    /// </summary>
    bool HasOrganization { get; }

    /// <summary>
    /// Проверяет, имеет ли текущий пользователь определенное разрешение в контексте своей организации.
    /// </summary>
    bool HasPermission(string permissionCode);


    /// <summary>
    /// Загружает контекст организации текущего пользователя.
    /// </summary>
    /// <exception cref="InvalidOperationException"> 
    /// Выбрасывается если текущей пользователь не аутентифицирован.
    /// </exception>
    Task LoadContext();
}
