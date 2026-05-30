using Microsoft.EntityFrameworkCore;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Domain;

namespace FinaccApi.Modules.Access.Infrastructure;

/// <summary>
/// Загружает и хранит контекст активной организации текущего пользователя.
/// </summary>
public class CurrentOrganization : ICurrentOrganization
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private bool _organizationContextLoaded;

    /// <summary>
    /// Создает сервис контекста организации.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных приложения.</param>
    /// <param name="currentUser">Сервис доступа к текущему пользователю.</param>
    public CurrentOrganization(ApplicationDbContext dbContext, ICurrentUser currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Идентификатор активной организации текущего пользователя, если контекст найден.
    /// </summary>
    public int? OrganizationId { get; private set; }

    /// <summary>
    /// Идентификатор активного членства пользователя в организации, если контекст найден.
    /// </summary>
    public int? MembershipId { get; private set; }

    /// <summary>
    /// Идентификатор роли пользователя в активной организации, если контекст найден.
    /// </summary>
    public int? RoleId { get; private set; }

    /// <summary>
    /// Коды разрешений, связанных с ролью пользователя в активной организации.
    /// </summary>
    public IReadOnlyList<string> Permissions { get; private set; } = [];

    /// <summary>
    /// Указывает, найдено ли активное членство текущего пользователя в организации.
    /// </summary>
    public bool HasOrganization { get; private set; }

    /// <summary>
    /// Проверяет наличие разрешения в загруженном контексте организации.
    /// </summary>
    /// <param name="permissionCode">Код проверяемого разрешения.</param>
    /// <returns>True, если разрешение присутствует в контексте; иначе false.</returns>
    public bool HasPermission(string permissionCode)
    {
        if (!_organizationContextLoaded)
            throw new InvalidOperationException("Organization context is not loaded.");

        return Permissions.Contains(permissionCode);
    }

    /// <summary>
    /// Загружает активное членство пользователя, его роль и список разрешений.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>Задача, представляющая асинхронную загрузку контекста.</returns>
    public async Task LoadContext(CancellationToken cancellationToken)
    {
        if (_organizationContextLoaded)
            return;

        if (!_currentUser.IsAuthenticated || _currentUser.UserId is not int userId)
        {
            MarkContextAsEmpty();
            return;
        }

        var membership = await _dbContext.OrganizationMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.UserId == userId && x.Status == OrganizationMemberStatus.Active,
                cancellationToken);

        if (membership == null)
        {
            MarkContextAsEmpty();
            return;
        }

        var rolePermissions = await _dbContext.RolePermissions
            .AsNoTracking()
            .Where(x => x.RoleId == membership.RoleId)
            .Select(x => x.Permission.Code)
            .ToListAsync(cancellationToken);

        HasOrganization = true;
        OrganizationId = membership.OrganizationId;
        MembershipId = membership.Id;
        RoleId = membership.RoleId;
        Permissions = rolePermissions;

        _organizationContextLoaded = true;
    }

    /// <summary>
    /// Сбрасывает контекст организации в пустое загруженное состояние.
    /// </summary>
    private void MarkContextAsEmpty()
    {
        HasOrganization = false;
        OrganizationId = null;
        MembershipId = null;
        RoleId = null;
        Permissions = [];
        _organizationContextLoaded = true;
    }
}
