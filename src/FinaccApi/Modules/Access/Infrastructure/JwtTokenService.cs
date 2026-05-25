using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FinaccApi.Modules.Access.Infrastructure;


public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

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
    
        // Получаем секретный ключ из конфигурации
        var jwtSettings = _configuration.GetSection("Jwt");
        var secretKeyString = jwtSettings["Secret"]!;

        // Конвертируем строку ключа в байты
        // JWT требует байты для подписи
        var secretKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKeyString)
        );

        // Создаем объект подписи с алгоритмом HMAC SHA256
        // Это гарантирует что никто не может подделать токен
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        // Описываем дескриптор токена 
        var tokenDescriptor = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        // Создаем и возращаем сам токен (строку)
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    public string GenerateAccessToken(int userId, string email)
    {
        return GetToken(userId, email, DateTime.UtcNow.AddMinutes(15));
    }

    public string GenerateRefreshToken(int userId, string email)
    {
        return GetToken(userId, email, DateTime.UtcNow.AddDays(7));
    }

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
            
            // Извлекаем userId из claims
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
