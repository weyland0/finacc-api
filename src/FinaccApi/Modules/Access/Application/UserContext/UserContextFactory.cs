using Microsoft.EntityFrameworkCore;
using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Domain;

namespace FinaccApi.Modules.Access.Application.UserContext;

public class UserContextFactory
{
    ApplicationDbContext _dbContext;

    public UserContextFactory(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserContextResponse> Create(User user, CancellationToken cancellationToken)
    {
        var organizationContext = await GetOrganizationContext(user.Id, cancellationToken);

        return new UserContextResponse
        {
            User = MapUser(user),
            Organization = organizationContext.Organization,
            Role = organizationContext.Role,
            Permissions = organizationContext.Permissions
        };
    }

    private static UserResponse MapUser(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Phone = user.Phone,
        FirstName = user.FirstName,
        LastName = user.LastName
    };

    private async Task<OrganizationContext> GetOrganizationContext(int userId, CancellationToken cancellationToken)
    {
        var membership = await _dbContext.OrganizationMembers
            .Where(x => x.UserId == userId && x.Status == OrganizationMemberStatus.Active)
            .Select(x => new { x.OrganizationId, x.RoleId })
            .FirstOrDefaultAsync(cancellationToken);

        if (membership is null)
            return OrganizationContext.Empty;

        var organization = await _dbContext.Organizations
            .Where(x => x.Id == membership.OrganizationId)
            .Select(x => new OrganizationResponse { Id = x.Id, Name = x.Name })
            .FirstOrDefaultAsync(cancellationToken);

        var role = await _dbContext.Roles
            .Where(x => x.Id == membership.RoleId)
            .Select(x => new RoleResponse { Id = x.Id, Name = x.Name, Code = x.Code })
            .FirstOrDefaultAsync(cancellationToken);

        var permissions = await _dbContext.RolePermissions
            .Where(x => x.RoleId == membership.RoleId)
            .Select(x => x.Permission.Code)
            .ToListAsync(cancellationToken);

        return new OrganizationContext(organization, role, permissions);
    }

    private sealed record OrganizationContext(
        OrganizationResponse? Organization,
        RoleResponse? Role,
        IList<string> Permissions)
    {
        public static OrganizationContext Empty { get; } = new(null, null, []);
    }
}
