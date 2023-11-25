using System.Net;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Utilities.JsonExceptions;

namespace Sharecode.Backend.Application.Exceptions;

[ExceptionDetail(34871, "The user has not yet registered with a password. He may have registered using a social login")]
public class PasswordNotYetGeneratedException : AppException
{
    public PasswordNotYetGeneratedException() : base($"No regular registration has been yet happened", 34871, HttpStatusCode.BadRequest)
    {
        SetMessage(
            $"This account was created with social auth, Please use social auth or forgot password");
    }
}

[ExceptionDetail(34872, "Obviously, Caught you..., Wrong password!")]
public class InvalidPasswordException : AppException
{
    public InvalidPasswordException() : base($"Invalid password!", 34872, HttpStatusCode.BadRequest)
    {
        SetMessage($"Invalid password");
    }
}

[ExceptionDetail(32172, $"The authentication service (Google, Facebook) provided invalid token or token doesn't validate the ownership of the account")]
public class InvalidAuthFromSocialException : AppException 
{
    public InvalidAuthFromSocialException(AuthorizationType type, string message) : base($"Failed to validate user {type}", 32172, HttpStatusCode.BadRequest)
    {
        SetMessage($"Failed to validate user {type}");
    }    
}