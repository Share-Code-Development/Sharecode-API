using System.Net;
using Sharecode.Backend.Domain.Entity.Profile;
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

[ExceptionDetail(1212367, "The account is suspended, Either check your email address or contact support!")]
public class AccountTemporarilySuspendedException : AppException
{
    public AccountTemporarilySuspendedException(string emailAddress, string reason) : base($"This account associated with the email {emailAddress} has been suspended", 1212367, HttpStatusCode.BadRequest)
    {
        SetMessage($"This account has been suspended. Contact support. Reason: {reason}");
    }
}

[ExceptionDetail(9999, "The account has been locked due to being multiple incorrect login attempts")]
public class AccountLockedException : AppException
{
    public AccountLockedException() : base($"The account has been locked due to being multiple incorrect login attempts. If you have access to the email address, please reset your password", 9999, HttpStatusCode.BadRequest)
    {
        SetMessage($"The account has been locked due to being multiple incorrect login attempts. If you have access to the email address, please reset your password");
    }
}

[ExceptionDetail(56521, "The action you are trying to perform require an elevated permission and the requesting user doesn't have that")]
public class NotEnoughPermissionException : AppException
{
    public NotEnoughPermissionException(string action, bool showPermission = false, params Permission[] permissions) : base($"The user doesn't have either any required permission or all of the required permission to execute {action}", 56521, HttpStatusCode.Forbidden)
    {
        if (showPermission)
        {
            SetMessage(
                $"Your account doesn't have either (all of) or (any of) the permission(s) required to perform {action}. Permission(s) [{string.Join(", ", permissions)}]");
        }
    }
}

[ExceptionDetail(817120, "The requested account is deleted in the system")]
public class AccountDeleteException : AppException
{
    public AccountDeleteException(Guid userId) : base($"This account has been deleted", 817120, HttpStatusCode.NotFound)
    {
    }
}