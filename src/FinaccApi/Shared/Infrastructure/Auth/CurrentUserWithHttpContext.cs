using System.Security.Claims;
using FinaccApi.Shared.Application.Auth;

namespace FinaccApi.Shared.Infrastructure.Auth;

/// <summary>
/// Получает данные текущего пользователя из claims активного HTTP-контекста.
/// </summary>
public class CurrentUserWithHttpContext : ICurrentUser
{
    /// <summary>
    /// Идентификатор текущего пользователя, если запрос аутентифицирован.
    /// </summary>
    public int? UserId { get => ParseUserIdFromClaims(); }

    /// <summary>
    /// Электронная почта текущего пользователя, если запрос аутентифицирован.
    /// </summary>
    public string? Email { get => ParseEmailFromClaims(); }

    /// <summary>
    /// Признак наличия аутентифицированного пользователя в текущем HTTP-контексте.
    /// </summary>
    public bool IsAuthenticated { get => CheckIsAuthenticated(); }

    /// <summary>
    /// Идентификатор текущего пользователя, если запрос аутентифицирован
    /// и он содержит корректный userId в claims.
    /// userId считается корректным, если он имеет тип int и занчение больше 0.
    /// </summary>
    public int RequiredUserId =>
        UserId ?? throw new InvalidOperationException("Authenticated user id claim is missing or invalid.");

    /// <summary>
    /// Идентификатор текущего пользователя, если запрос аутентифицирован
    /// и он содержит корректный email в claims.
    /// </summary>
    public string RequiredEmail =>
        Email ?? throw new InvalidOperationException("Authenticated user email claim is missing.");


    /// <summary>
    /// Аксессор текущего HTTP-контекста.
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Создает сервис доступа к данным текущего пользователя.
    /// </summary>
    /// <param name="httpContextAccessor">Аксессор текущего HTTP-контекста.</param>
    public CurrentUserWithHttpContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Извлекает идентификатор пользователя из claims текущего запроса.
    /// </summary>
    /// <returns>Идентификатор пользователя, если он найден и успешно преобразован; иначе null.</returns>
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
    /// Извлекает электронную почту пользователя из claims текущего запроса.
    /// </summary>
    /// <returns>Электронная почта пользователя, если она найдена; иначе null.</returns>
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
    /// Проверяет, содержит ли текущий HTTP-контекст аутентифицированного пользователя.
    /// </summary>
    /// <returns>True, если пользователь аутентифицирован; иначе false.</returns>
    private bool CheckIsAuthenticated()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.Identity?.IsAuthenticated == true;
    }
}
