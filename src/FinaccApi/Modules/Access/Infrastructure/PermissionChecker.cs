using FinaccApi.Shared.Application.Auth;
using FinaccApi.Shared.Application.MultiTenancy;


namespace FinaccApi.Modules.Access.Infrastructure;


public class PermissionChecker : IPermissionChecker
{
    ICurrentOrganization _currentOrganization;

    public PermissionChecker(ICurrentOrganization currentOrganization)
    {
        _currentOrganization = currentOrganization;
    }

    public async Task<bool> HasPermission(string permissionCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(permissionCode))
            throw new ArgumentException("Permission code must be not empty.");

        if (!_currentOrganization.HasOrganization)
            return false;

        await _currentOrganization.LoadContext();

        return _currentOrganization.HasPermission(permissionCode);
    }
}
