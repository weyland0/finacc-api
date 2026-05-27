namespace FinaccApi.Shared.Infrastructure.Jwt;

/// <summary>
/// Настройки для JWT, загружаемые из конфигурации приложения.
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    /// Имя секции в конфигурации.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Издатель токена.
    /// </summary>
    public string Issuer { get; init; } = null!;

    /// <summary>
    /// Аудитория токена.
    /// </summary>
    public string Audience { get; init; } = null!;
    
    /// <summary>
    /// Секретный ключ для подписи токена.
    /// </summary>
    public string Secret { get; init; } = null!;

    /// <summary>
    /// Время жизни токена в минутах.
    /// </summary>
    public int AccessTokenLifetimeMinutes { get; init; }
}
