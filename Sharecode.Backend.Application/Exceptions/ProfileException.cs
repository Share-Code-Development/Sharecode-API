using System.Net;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Utilities.JsonExceptions;

namespace Sharecode.Backend.Application.Exceptions;

[ExceptionDetail(340912, "The profile that is requested is private and you are not allowed to access it!")]
public class ProfileIsPrivateException : AppException
{
    public ProfileIsPrivateException() : base("The requested profile is private",340912, HttpStatusCode.NoContent)
    {
        SetMessage("The requested profile is private");
    }
}

[ExceptionDetail(123167, $"The account is registered but the user has not verified the account yet!")]
public class EmailNotVerifiedException : AppException
{
    
    public EmailNotVerifiedException(string emailAddress, string reason) : base(reason, 123167, HttpStatusCode.BadRequest)
    {
        SetMessage(reason);
    }
}

[ExceptionDetail(1212367, "The account is locked, Either check your email address or contact support!")]
public class AccountLockedException : AppException
{
    public AccountLockedException(string emailAddress, string reason) : base($"This account has been locked", 1212367, HttpStatusCode.BadRequest)
    {
        SetMessage($"This account has been locked. Contact support. Reason: {reason}");
    }
}