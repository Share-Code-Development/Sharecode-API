using System.Net;
using Sharecode.Backend.Domain.Exceptions;

namespace Sharecode.Backend.Application.Exceptions;

public class ProfileIsPrivateException : AppException
{
    public ProfileIsPrivateException() : base("The requested profile is private",340912, HttpStatusCode.NoContent)
    {
        SetMessage("The requested profile is private");
    }
}

public class EmailNotVerifiedException : AppException
{
    
    public EmailNotVerifiedException(string emailAddress, string reason) : base(reason, 123167, HttpStatusCode.BadRequest)
    {
        SetMessage(reason);
    }
}

public class AccountLockedException : AppException
{
    public AccountLockedException(string emailAddress, string reason) : base($"This account has been locked", 1212367, HttpStatusCode.BadRequest)
    {
        SetMessage($"This account has been locked. Contact support. Reason: {reason}");
    }
}