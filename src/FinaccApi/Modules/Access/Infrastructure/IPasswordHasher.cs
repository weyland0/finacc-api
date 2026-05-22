namespace FinaccApi.Modules.Access.Infrastructure;

/// <summary>
/// Интерфейс модуля хэширования паролей.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Хэширует пароль, введеный пользователем.
    /// </summary>
    /// <param name="password"> Пароль </param>
    /// <returns> Хеш пароля </returns>
    string Hash(string password);

    /// <summary>
    /// Проверяет, соответствует ли введенный пользователем пароль
    /// действительному паролю, сравнивая их хеши.
    /// </summary>
    /// <param name="password"> Введеный пароль </param>
    /// <param name="hash"> Хеш дейсвтительного пароля </param>
    bool Verify(string password, string hash);
}
