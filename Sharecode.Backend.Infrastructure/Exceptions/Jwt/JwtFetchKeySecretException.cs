namespace Sharecode.Backend.Infrastructure.Exceptions.Jwt;

public class JwtFetchKeySecretException : Exception
{

    public JwtFetchKeySecretException(string? message) : base(message)
    {
    }
}