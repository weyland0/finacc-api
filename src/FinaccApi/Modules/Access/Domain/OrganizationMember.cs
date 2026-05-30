using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Access.Domain;

/// <summary>
/// Представляет членство пользователя в организации и его роль внутри нее.
/// </summary>
public class OrganizationMember : AuditableEntity, IOrganizationOwnedEntity
{
    /// <summary>
    /// Идентификатор пользователя, которому принадлежит членство.
    /// </summary>
    public int UserId { get; private set; }

    /// <summary>
    /// Идентификатор организации, в которой состоит пользователь.
    /// </summary>
    public int OrganizationId { get; private set; }

    /// <summary>
    /// Идентификатор роли пользователя в организации.
    /// </summary>
    public int RoleId { get; private set; }

    /// <summary>
    /// Код текущего статуса участника организации.
    /// </summary>
    public string Status { get; private set; } = null!;


    /// <summary>
    /// Создает активное членство пользователя в организации.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="roleId">Идентификатор роли пользователя в организации.</param>
    public OrganizationMember(int userId, int organizationId, int roleId)
    {
        UserId = userId;
        OrganizationId = organizationId;
        RoleId = roleId;
        Status = OrganizationMemberStatus.Active;
    }

    /// <summary>
    /// Создает членство пользователя в организации с указанным статусом.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="roleId">Идентификатор роли пользователя в организации.</param>
    /// <param name="status">Код статуса участника организации.</param>
    public OrganizationMember(int userId, int organizationId, int roleId, string status)
    {
        UserId = userId;
        OrganizationId = organizationId;
        RoleId = roleId;
        Status = status;
    }
}
