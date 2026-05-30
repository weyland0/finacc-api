namespace FinaccApi.Shared.Application.Auth;

/// <summary>
/// Проверяет права доступа текущего пользователя.
/// </summary>
public interface IPermissionChecker
{
    /// <summary>
    /// Проверяет, есть ли у текущего пользователя указанное разрешение.
    /// </summary>
    /// <param name="permissionCode">Код проверяемого разрешения.</param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>True, если разрешение есть; иначе false.</returns>
    Task<bool> HasPermission(string permissionCode, CancellationToken cancellationToken);
}
