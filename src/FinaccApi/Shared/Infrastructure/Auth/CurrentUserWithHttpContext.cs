using System.Security.Claims;
using FinaccApi.Shared.Application.Auth;

namespace FinaccApi.Shared.Infrastructure.Auth;

/// <summary>
/// Реализация интерфейса ICurrentUser, которая использует HttpContext
/// для получения информации о текущем пользователе.
/// </summary>
public class CurrentUserWithHttpContext : ICurrentUser
{
    /// <summary>
    /// Идентификатор текущего пользователя, если он аутентифицирован, иначе null.
    /// </summary>
    public int? UserId { get => ParseUserIdFromClaims(); }

    /// <summary>
    /// Электронная почта текущего пользователя, если он аутентифицирован, иначе null.
    /// </summary>
    public string? Email { get => ParseEmailFromClaims(); }

    /// <summary>
    /// Признак того, что текущий пользователь аутентифицирован.
    /// </summary>
    public bool IsAuthenticated { get => CheckIsAuthenticated(); }

    /// <summary>
    /// HttpContextAccessor для доступа к текущему HTTP-контексту,
    /// который содержит информацию о пользователе и его аутентификации.
    /// </summary>
    private IHttpContextAccessor _httpContextAccessor;

    public CurrentUserWithHttpContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Извлекает идентификатор пользователя из его claims (утверждений).
    /// </summary>
    /// <returns> Идентификатор пользователя, если он был найден, иначе null. </returns>
    private int? ParseUserIdFromClaims()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var userIdClaim = user.FindFirst("userId")?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return int.TryParse(userIdClaim, out var userId)
            ? userId : null;
    }

    /// <summary>
    /// Извлекает email пользователя из его claims (утверждений).
    /// </summary>
    /// <returns> Email пользователя, если он был найден, иначе null. </returns>
    private string? ParseEmailFromClaims()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        return user?.FindFirst(ClaimTypes.Email)?.Value
            ?? user?.FindFirst("email")?.Value;
    }

    /// <summary>
    /// Проверяет, аутентифицирован ли текущий пользователь, 
    /// основываясь на его claims (утверждениях).
    /// </summary>
    private bool CheckIsAuthenticated()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.Identity?.IsAuthenticated == true;
    }
}
