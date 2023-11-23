using System.Security.Cryptography;
using System.Text;

namespace Sharecode.Backend.Utilities.SecurityClient;

public class SecurityClient : ISecurityClient
{

    public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] salt)
    {
        using var hmac = new HMACSHA512();
        salt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
    
    public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] salt)
    {
        using var hmac = new HMACSHA512(salt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return passwordHash.SequenceEqual(computedHash);
    }
}