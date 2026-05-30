using System.Text;
using System.Security.Cryptography;

namespace FinaccApi.Modules.Access.Infrastructure;

/// <summary>
/// Хеширует и проверяет пароли с помощью алгоритма SHA-256.
/// </summary>
public class PasswordHasherSha256 : IPasswordHasher
{
    /// <summary>
    /// Вычисляет Base64-представление SHA-256 хеша пароля.
    /// </summary>
    /// <param name="password">Пароль в открытом виде.</param>
    /// <returns>Хеш пароля в формате Base64.</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если пароль пустой.</exception>
    public string Hash(string password)
    {
        if (password.Length == 0)
        {
            throw new ArgumentException("The password must be longer than zero!");
        }

        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    /// <summary>
    /// Проверяет, соответствует ли пароль ранее вычисленному хешу.
    /// </summary>
    /// <param name="password">Пароль в открытом виде.</param>
    /// <param name="hash">Ожидаемый хеш пароля в формате Base64.</param>
    /// <returns>True, если хеш пароля совпадает с ожидаемым хешем; иначе false.</returns>
    public bool Verify(string password, string hash)
    {
        if (password.Length == 0 && hash.Length != 0)
            return false;
        if (hash.Length == 0)
            return false;

        var hashOfInput = Hash(password);
        return hashOfInput.Equals(hash);
    }
}
