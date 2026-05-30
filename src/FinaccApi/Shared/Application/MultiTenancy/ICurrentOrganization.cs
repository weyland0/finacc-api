namespace FinaccApi.Shared.Application.MultiTenancy;

/// <summary>
/// Предоставляет контекст организации, связанной с текущим пользователем.
/// </summary>
public interface ICurrentOrganization
{
    /// <summary>
    /// Идентификатор активной организации текущего пользователя, если контекст найден.
    /// </summary>
    int? OrganizationId { get; }

    /// <summary>
    /// Идентификатор активного членства пользователя в организации, если контекст найден.
    /// </summary>
    int? MembershipId { get; }

    /// <summary>
    /// Идентификатор роли пользователя в активной организации, если контекст найден.
    /// </summary>
    int? RoleId { get; }

    /// <summary>
    /// Коды разрешений, связанных с ролью пользователя в активной организации.
    /// </summary>
    IReadOnlyList<string> Permissions { get; }

    /// <summary>
    /// Указывает, найдено ли активное членство текущего пользователя в организации.
    /// </summary>
    bool HasOrganization { get; }

    /// <summary>
    /// Проверяет наличие разрешения у текущего пользователя в загруженном контексте активной организации.
    /// </summary>
    /// <param name="permissionCode">Код проверяемого разрешения.</param>
    /// <returns>True, если разрешение есть в текущем контексте организации; иначе false.</returns>
    /// <exception cref="InvalidOperationException">
    /// Выбрасывается, если контекст организации еще не был загружен.
    /// </exception>
    bool HasPermission(string permissionCode);


    /// <summary>
    /// Загружает контекст активной организации текущего пользователя.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>Задача, представляющая асинхронную загрузку контекста.</returns>
    Task LoadContext(CancellationToken cancellationToken);
}
