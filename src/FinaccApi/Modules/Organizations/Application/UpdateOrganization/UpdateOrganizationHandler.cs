using FinaccApi.Shared.Kernel;
using FinaccApi.DataAccess;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Modules.Access.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinaccApi.Modules.Organizations.Application.UpdateOrganization;


public class UpdateOrganizationHandler
{
    ApplicationDbContext _dbContext;
    ICurrentOrganization _currentOrganization;
    IPermissionChecker _permissionChecker;

    public UpdateOrganizationHandler(
        ApplicationDbContext dbContext,
        ICurrentOrganization currentOrganization,
        IPermissionChecker permissionChecker)
    {
        _dbContext = dbContext;
        _currentOrganization = currentOrganization;
        _permissionChecker = permissionChecker;
    }

    public async Task<Result<OrganizationResponse>> Handle(
        UpdateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        await _currentOrganization.LoadContext(cancellationToken);

        if (!_currentOrganization.HasOrganization || _currentOrganization.OrganizationId == null)
            return Result.Failure<OrganizationResponse>(OrganizationErrors.OrganizationNotFound);

        var permissionResult = await _permissionChecker.HasPermission(
            Permissions.Organization.Update, cancellationToken);

        if (!permissionResult)
        {
            return Result.Failure<OrganizationResponse>(OrganizationErrors.Forbidden);
        }

        var organization = await _dbContext.Organizations.SingleOrDefaultAsync(
            x => x.Id == _currentOrganization.OrganizationId, cancellationToken);

        if (organization == null)
            return Result.Failure<OrganizationResponse>(OrganizationErrors.OrganizationNotFound);

        organization.Update(
            command.Name,
            command.LegalEntityName,
            command.RegistrationNumber,
            command.TaxId,
            command.FullAddress,
            command.Email,
            command.Phone);

        await _dbContext.SaveChangesAsync();

        return Result.Success(OrganizationResponse.CreateFromOrganization(organization));
    }
}
