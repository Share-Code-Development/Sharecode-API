using System.Net;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Utilities.JsonExceptions;

namespace Sharecode.Backend.Api.Exceptions;

[ExceptionDetail(errorCode: 50000, errorDescription: "An internal server error which tries to access the cache prematurely or developers messed up haha!")]
public class InvalidCacheAccessException : AppException
{
    public InvalidCacheAccessException(string endpoint, bool set) : base(
        $"An endpoint has been trying to "+(set ? "set" : "get")+$" the cache on {endpoint} while the request is not yet mature",
        (long)50000,
        HttpStatusCode.InternalServerError)
    {
        SetMessage("An unknown error occured while accessing your data! Please try again later");
    }
    
    public InvalidCacheAccessException(string[] keys, string endpoint) : base(
        $"Tried to create an invalid cache key without minimum block on {endpoint}. Keys are {string.Join("-", keys)}",
        (long)50000,
        HttpStatusCode.InternalServerError)
    {
        SetMessage("An unknown error occured while accessing your data! Please try again later");
    }
}