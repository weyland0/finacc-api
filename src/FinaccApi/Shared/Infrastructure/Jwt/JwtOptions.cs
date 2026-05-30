namespace FinaccApi.Shared.Infrastructure.Jwt;

/// <summary>
/// Настройки JWT, загружаемые из конфигурации приложения.
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    /// Имя секции конфигурации, содержащей настройки JWT.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Значение issuer, указывающее издателя токена.
    /// </summary>
    public string Issuer { get; init; } = null!;

    /// <summary>
    /// Значение audience, указывающее получателя токена.
    /// </summary>
    public string Audience { get; init; } = null!;
    
    /// <summary>
    /// Секретный ключ, используемый для подписи JWT.
    /// </summary>
    public string Secret { get; init; } = null!;

    /// <summary>
    /// Время жизни access token в минутах.
    /// </summary>
    public int AccessTokenLifetimeMinutes { get; init; }
}
