using FinaccApi.Shared.Application.Auth;
using FinaccApi.Shared.Application.MultiTenancy;


namespace FinaccApi.Modules.Access.Infrastructure;

/// <summary>
/// Проверяет права текущего пользователя через контекст активной организации.
/// </summary>
public class PermissionChecker : IPermissionChecker
{
    ICurrentOrganization _currentOrganization;

    /// <summary>
    /// Создает сервис проверки разрешений.
    /// </summary>
    /// <param name="currentOrganization">Контекст активной организации текущего пользователя.</param>
    public PermissionChecker(ICurrentOrganization currentOrganization)
    {
        _currentOrganization = currentOrganization;
    }

    /// <summary>
    /// Проверяет, доступно ли текущему пользователю указанное разрешение.
    /// </summary>
    /// <param name="permissionCode">Код проверяемого разрешения.</param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>True, если разрешение доступно текущему пользователю; иначе false.</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если код разрешения пустой.</exception>
    public async Task<bool> HasPermission(string permissionCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(permissionCode))
            throw new ArgumentException("Permission code must be not empty.");

        await _currentOrganization.LoadContext();

        if (!_currentOrganization.HasOrganization)
            return false;

        return _currentOrganization.HasPermission(permissionCode);
    }
}
