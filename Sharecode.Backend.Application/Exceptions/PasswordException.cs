using System.Net;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Exceptions;

namespace Sharecode.Backend.Application.Exceptions;

public class PasswordNotYetGeneratedException : AppException
{
    public PasswordNotYetGeneratedException() : base($"No regular registration has been yet happened", 34871, HttpStatusCode.BadRequest)
    {
        SetMessage(
            $"This account was created with social auth, Please use social auth or forgot password");
    }
}

public class InvalidPasswordException : AppException
{
    public InvalidPasswordException() : base($"Invalid password!", 34872, HttpStatusCode.BadRequest)
    {
        SetMessage($"Invalid password");
    }
}

public class InvalidAuthFromSocialException : AppException 
{
    public InvalidAuthFromSocialException(AuthorizationType type, string message) : base($"Failed to validate user {type}", 32172, HttpStatusCode.BadRequest)
    {
        SetMessage($"Failed to validate user {type}");
    }    
}