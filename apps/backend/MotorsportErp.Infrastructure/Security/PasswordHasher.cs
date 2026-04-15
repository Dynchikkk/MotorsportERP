using MotorsportErp.Application.Common.Interfaces.Security;
using System.Security.Cryptography;

namespace MotorsportErp.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128 bit
    private const int KeySize = 32;  // 256 bit
    private const int Iterations = 10000;

    public string Hash(string password)
    {
        using var algorithm = new Rfc2898DeriveBytes(
            password,
            SaltSize,
            Iterations,
            HashAlgorithmName.SHA256);

        var salt = algorithm.Salt;
        var key = algorithm.GetBytes(KeySize);

        var result = new byte[SaltSize + KeySize];

        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(key, 0, result, SaltSize, KeySize);

        return Convert.ToBase64String(result);
    }

    public bool Verify(string password, string hash)
    {
        var bytes = Convert.FromBase64String(hash);

        var salt = new byte[SaltSize];
        var key = new byte[KeySize];

        Buffer.BlockCopy(bytes, 0, salt, 0, SaltSize);
        Buffer.BlockCopy(bytes, SaltSize, key, 0, KeySize);

        using var algorithm = new Rfc2898DeriveBytes(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256);

        var keyToCheck = algorithm.GetBytes(KeySize);

        return CryptographicOperations.FixedTimeEquals(keyToCheck, key);
    }
}