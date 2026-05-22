using System.Text;
using System.Security.Cryptography;

namespace FinaccApi.Modules.Access.Infrastructure;

public class PasswordHasherSha256 : IPasswordHasher
{
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
