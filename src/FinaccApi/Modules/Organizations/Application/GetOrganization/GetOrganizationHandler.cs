using FinaccApi.Shared.Kernel;
using FinaccApi.DataAccess;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Modules.Access.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinaccApi.Modules.Organizations.Application.GetOrganization;


public class GetOrganizationHandler
{
    ApplicationDbContext _dbContext;
    ICurrentOrganization _currentOrganization;
    IPermissionChecker _permissionChecker;

    public GetOrganizationHandler(
        ApplicationDbContext dbContext,
        ICurrentOrganization currentOrganization,
        IPermissionChecker permissionChecker)
    {
        _dbContext = dbContext;
        _currentOrganization = currentOrganization;
        _permissionChecker = permissionChecker;
    }

    public async Task<Result<OrganizationResponse>> Handle(CancellationToken cancellationToken)
    {
        await _currentOrganization.LoadContext(cancellationToken);

        if (!_currentOrganization.HasOrganization || _currentOrganization.OrganizationId == null)
            return Result.Failure<OrganizationResponse>(OrganizationErrors.OrganizationNotFound);

        var permissionResult = await _permissionChecker.HasPermission(
            Permissions.Organization.View, cancellationToken);

        if (!permissionResult)
        {
            return Result.Failure<OrganizationResponse>(OrganizationErrors.Forbidden);
        }

        var organization = await _dbContext.Organizations.SingleOrDefaultAsync(
            x => x.Id == _currentOrganization.OrganizationId, cancellationToken);

        if (organization == null)
            return Result.Failure<OrganizationResponse>(OrganizationErrors.OrganizationNotFound);

        return Result.Success(OrganizationResponse.CreateFromOrganization(organization));
    }
}
