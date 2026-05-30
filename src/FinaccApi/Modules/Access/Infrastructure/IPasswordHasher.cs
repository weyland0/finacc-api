namespace FinaccApi.Modules.Access.Infrastructure;

/// <summary>
/// Определяет операции хеширования и проверки паролей.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Вычисляет хеш пароля.
    /// </summary>
    /// <param name="password">Пароль в открытом виде.</param>
    /// <returns>Хеш пароля.</returns>
    string Hash(string password);

    /// <summary>
    /// Проверяет, соответствует ли пароль сохраненному хешу.
    /// </summary>
    /// <param name="password">Пароль в открытом виде.</param>
    /// <param name="hash">Сохраненный хеш пароля.</param>
    /// <returns>True, если пароль соответствует хешу; иначе false.</returns>
    bool Verify(string password, string hash);
}
