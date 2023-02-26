using System;
using Audiochan.Common.Exceptions;
using Audiochan.Common.Models;

namespace Audiochan.API.Errors;

public class UnauthorizedError : IUserError
{
    public static UnauthorizedError CreateErrorFrom(UnauthorizedAccessException exception)
    {
        return new UnauthorizedError();
    }
    
    public static UnauthorizedError CreateErrorFrom(ResourceIdInvalidException<long> exception)
    {
        return new UnauthorizedError();
    }
    
    public static UnauthorizedError CreateErrorFrom(ResourceOwnershipException<long> exception)
    {
        return new UnauthorizedError();
    }

    public string Code => GetType().Name;
    public string Message => "You are unauthorized access.";
}