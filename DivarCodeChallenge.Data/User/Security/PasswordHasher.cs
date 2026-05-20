using System.Security.Cryptography;
using System.Text;
using DivarCodeChallenge.Application.Users.Interfaces;

namespace DivarCodeChallenge.Infrastructure.User.Security;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            10000,
            HashAlgorithmName.SHA256);

        byte[] hash = pbkdf2.GetBytes(32);

        return Convert.ToBase64String(salt) + "." +
               Convert.ToBase64String(hash);
    }

    public bool Verify(string password, string storedHash)
    {
        var parts = storedHash.Split('.');

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] hash = Convert.FromBase64String(parts[1]);

        var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            10000,
            HashAlgorithmName.SHA256);

        byte[] computedHash = pbkdf2.GetBytes(32);

        return CryptographicOperations.FixedTimeEquals(hash, computedHash);
    }
}
