using System.Net;
using Sharecode.Backend.Domain.Exceptions;

namespace Sharecode.Backend.Api.Exceptions;

public class InvalidCacheAccessException : AppException
{
    public InvalidCacheAccessException(string endpoint, bool set) : base(
        $"An endpoint has been trying to "+(set ? "set" : "get")+$" the cache on {endpoint} while the request is not yet mature",
        (long)50000,
        HttpStatusCode.InternalServerError)
    {
        SetMessage("An unknown error occured while accessing your data! Please try again later");
    }
}