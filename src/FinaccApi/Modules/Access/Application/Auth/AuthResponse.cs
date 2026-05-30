namespace FinaccApi.Modules.Access.Application.Auth;

/// <summary>
/// Данные, возвращаемые после успешной аутентификации пользователя.
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Access token для обращения к защищенным ресурсам API.
    /// </summary>
    public string AccessToken { get; set; } = null!;

    /// <summary>
    /// Профиль аутентифицированного пользователя.
    /// </summary>
    public UserResponse User { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public OrganizationResponse? Organization { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public RoleResponse? Role { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public IList<string> Permissions { get; set; } = [];
}
