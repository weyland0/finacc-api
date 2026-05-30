using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FinaccApi.Modules.Access.Infrastructure;

/// <summary>
/// Создает и проверяет JWT-токены для пользователей модуля доступа.
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Создает сервис генерации JWT-токенов.
    /// </summary>
    /// <param name="configuration">Конфигурация приложения с настройками JWT.</param>
    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Создает JWT с указанным сроком действия.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя, для которого создается токен.</param>
    /// <param name="email">Электронная почта пользователя.</param>
    /// <param name="expiresAt">Дата и время истечения срока действия токена в UTC.</param>
    /// <returns>JWT в строковом представлении.</returns>
    /// <exception cref="ArgumentException">
    /// Выбрасывается, если идентификатор пользователя некорректен или email пустой.
    /// </exception>
    private string GetToken(int userId, string email, DateTime expiresAt)
    {
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer!", nameof(userId));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email must not be empty!", nameof(email));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
        };
    
        // Получаем секретный ключ из конфигурации.
        var jwtSettings = _configuration.GetSection("Jwt");
        var secretKeyString = jwtSettings["Secret"]!;

        // Преобразуем строку ключа в байты для подписи JWT.
        var secretKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKeyString)
        );

        // Создаем подпись HMAC SHA256, чтобы токен нельзя было подделать без секрета.
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        // Описываем дескриптор токена.
        var tokenDescriptor = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        // Создаем и возвращаем токен в строковом представлении.
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    /// <summary>
    /// Генерирует краткоживущий access token для доступа к защищенным ресурсам API.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="email">Электронная почта пользователя.</param>
    /// <returns>JWT access token.</returns>
    public string GenerateAccessToken(int userId, string email)
    {
        return GetToken(userId, email, DateTime.UtcNow.AddMinutes(15));
    }

    /// <summary>
    /// Генерирует refresh token для последующего обновления access token.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="email">Электронная почта пользователя.</param>
    /// <returns>JWT refresh token.</returns>
    public string GenerateRefreshToken(int userId, string email)
    {
        return GetToken(userId, email, DateTime.UtcNow.AddDays(7));
    }

    /// <summary>
    /// Проверяет refresh token и извлекает идентификатор пользователя.
    /// </summary>
    /// <param name="refreshToken">Refresh token, полученный от клиента.</param>
    /// <returns>Идентификатор пользователя, если токен действителен; иначе null.</returns>
    public int? ValidateRefreshToken(string refreshToken)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKeyString = jwtSettings["Secret"]!;
            
            var secretKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKeyString)
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = secretKey
            };

            var principal = tokenHandler.ValidateToken(refreshToken, validationParameters, out var validatedToken);
            
            // Извлекаем идентификатор пользователя из claims.
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
