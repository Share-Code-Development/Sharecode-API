namespace Sharecode.Backend.Infrastructure.Exceptions.Jwt;

public class JwtGenerationException : Exception
{
    public JwtGenerationException(string? message) : base(message)
    {
    }
}