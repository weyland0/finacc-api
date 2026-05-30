namespace FinaccApi.Shared.Application.Auth;

public interface IPermissionChecker
{
    Task<bool> HasPermission(string permissionCode, CancellationToken cancellationToken);
}
