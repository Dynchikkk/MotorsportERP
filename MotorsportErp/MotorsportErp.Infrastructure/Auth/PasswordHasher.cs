using MotorsportErp.Application.Interfaces.Security;
using System.Security.Cryptography;
using System.Text;

namespace MotorsportErp.Infrastructure.Auth;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);

        return Convert.ToBase64String(hash);
    }

    public bool Verify(string password, string hash)
    {
        var hashedInput = Hash(password);
        return hashedInput == hash;
    }
}