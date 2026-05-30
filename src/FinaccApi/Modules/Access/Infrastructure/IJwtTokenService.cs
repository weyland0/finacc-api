namespace FinaccApi.Modules.Access.Infrastructure;

/// <summary>
/// Определяет операции генерации и проверки JWT-токенов.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Генерирует access token для доступа к защищенным ресурсам API.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="email">Электронная почта пользователя.</param>
    /// <returns>JWT access token.</returns>
    string GenerateAccessToken(int userId, string email);

    /// <summary>
    /// Генерирует refresh token для получения нового access token.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="email">Электронная почта пользователя.</param>
    /// <returns>JWT refresh token.</returns>
    string GenerateRefreshToken(int userId, string email);

    /// <summary>
    /// Проверяет refresh token и возвращает идентификатор пользователя.
    /// </summary>
    /// <param name="refreshToken">Refresh token, полученный от клиента.</param>
    /// <returns>Идентификатор пользователя, если токен действителен; иначе null.</returns>
    int? ValidateRefreshToken(string refreshToken);
}
