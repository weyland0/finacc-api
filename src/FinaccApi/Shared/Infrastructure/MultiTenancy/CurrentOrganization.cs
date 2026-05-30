using Microsoft.EntityFrameworkCore;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Domain;
using System.Threading.Tasks;

namespace FinaccApi.Shared.Infrastructure.MultiTenancy;

/// <summary>
/// Интерфейс для получения информации о текущей организации, к которой принадлежит пользователь.
/// </summary>
public class CurrentOrganization : ICurrentOrganization
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private bool _organizationContextLoaded = false;

    public CurrentOrganization(ApplicationDbContext dbContext, ICurrentUser currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Идентификатор организации, к которой принадлежит текущий пользователь.
    /// </summary>
    public int? OrganizationId { get; private set; }

    /// <summary>
    /// Идентификатор членства пользователя в организации, если применимо.
    /// </summary>
    public int? MembershipId { get; private set; }

    /// <summary>
    /// Идентификатор роли пользователя в организации, если применимо.
    /// </summary>
    public int? RoleId { get; private set; }

    /// <summary>
    /// Набор разрешений, связанных с ролью пользователя в организации.
    /// </summary>
    public IReadOnlyList<string> Permissions { get; private set; } = [];

    /// <summary>
    /// Указывает, принадлежит ли текущий пользователь какой-либо организации.
    /// </summary>
    public bool HasOrganization { get; private set; }

    /// <summary>
    /// Проверяет, имеет ли текущий пользователь определенное разрешение в контексте своей организации.
    /// </summary>
    public bool HasPermission(string permission)
    {
        return Permissions.Contains(permission);
    }

    /// <summary>
    /// Загружает контекст организации текущего пользователя.
    /// </summary>
    /// <exception cref="InvalidOperationException"> 
    /// Выбрасывается если текущей пользователь не аутентифицирован.
    /// </exception>
    public async Task LoadContext()
    {
        if (_organizationContextLoaded)
            return;

        if (!_currentUser.IsAuthenticated)
            throw new InvalidOperationException("User is not authenticated");

        var membership = await _dbContext.OrganizationMembers.FirstOrDefaultAsync(
            x => x.UserId == _currentUser.UserId!.Value);

        if (membership == null)
        {
            HasOrganization = false;
            return;
        }

        var rolePermissions = await _dbContext.RolePermissions
            .Where(x => x.RoleId == membership.RoleId)
            .Select(x => x.Permission.Code)
            .ToListAsync();

        HasOrganization = true;
        OrganizationId = membership.OrganizationId;
        MembershipId = membership.Id;
        RoleId = membership.RoleId;
        Permissions = rolePermissions;

        _organizationContextLoaded = true;
    }
}
