namespace FinaccApi.Modules.Access.Infrastructure;

/// <summary>
/// Интерфейс модуля для работы с JWT.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Генерирует Access Token (короткоживущий - 15 мин),
    /// который используется для доступа к защищенным ресурсам API
    /// </summary>
    /// <param name="userId"> Id пользователя </param>
    /// <param name="email"> Email пользователя </param>
    /// <returns> Access Token </returns>
    string GenerateAccessToken(int userId, string email);

    /// <summary>
    /// Генерирует Refresh Token (долгоживущий - 7 дней),
    /// котороый используется для получения нового Access Token
    /// </summary>
    /// <param name="userId"> Id пользователя </param>
    /// <param name="email"> Email пользователя </param>
    /// <returns> Refresh Token </returns>
    string GenerateRefreshToken(int userId, string email);

    /// <summary>
    /// Проверяет валидность Refresh Token и возвращает userId,
    /// если токен действителен.
    /// </summary>
    /// <param name="refreshToken"> Refresh Token </param>
    /// <returns> UserId </returns>
    int? ValidateRefreshToken(string refreshToken);
}
