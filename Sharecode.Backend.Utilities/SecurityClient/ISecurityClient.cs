namespace Sharecode.Backend.Utilities.SecurityClient;

public interface ISecurityClient
{
    void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] salt);
    bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] salt);
}