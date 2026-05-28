using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Access.Domain;

/// <summary>
/// Модель участника организации.
/// </summary>
public class OrganizationMember : AuditableEntity, IOrganizationOwnedEntity
{
    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public int UserId { get; private set; }

    /// <summary>
    /// Идентификатор организации.
    /// </summary>
    public int OrganizationId { get; private set; }

    /// <summary>
    /// Идентификатор роли.
    /// </summary>
    public int RoleId { get; private set; }

    /// <summary>
    /// Статус участника. 
    /// (Active / Removed / Left / Suspended)
    /// </summary>
    public string Status { get; private set; } = null!;


    /// <summary>
    /// Конструктор для создания участника организации.
    /// </summary>
    public OrganizationMember(int userId, int organizationId, int roleId)
    {
        UserId = userId;
        OrganizationId = organizationId;
        RoleId = roleId;
        Status = OrganizationMemberStatus.Active;
    }

    /// <summary>
    /// Конструктор для создания участника организации с указанием статуса.
    /// </summary>
    public OrganizationMember(int userId, int organizationId, int roleId, string status)
    {
        UserId = userId;
        OrganizationId = organizationId;
        RoleId = roleId;
        Status = status;
    }
}
