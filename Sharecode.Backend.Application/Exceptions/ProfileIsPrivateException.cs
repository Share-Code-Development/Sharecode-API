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