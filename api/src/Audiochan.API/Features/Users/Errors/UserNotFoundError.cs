using System;
using Audiochan.API.Features.Audios.Errors;
using Audiochan.Common.Exceptions;
using Audiochan.Common.Models;
using Audiochan.Core.Entities;
using HotChocolate.Types.Relay;
using HotChocolate.Utilities;

namespace Audiochan.API.Features.Users.Errors;

public class UserNotFoundError : IUserError
{
    public UserNotFoundError(long id)
    {
        UserId = id;
    }

    public static UserNotFoundError? CreateErrorFrom(Exception exception)
    {
        if (exception is ResourceIdInvalidException<long> invalidEx 
            && invalidEx.Resource.EqualsOrdinal(nameof(User)))
        {
            return new UserNotFoundError(invalidEx.Id);
        }

        if (exception is ResourceOwnershipException<long> forbidEx
            && forbidEx.Resource.EqualsOrdinal(nameof(User)))
        {
            return new UserNotFoundError(forbidEx.Id);
        }

        return null;
    }

    [ID(nameof(User))]
    public long UserId { get; }
    
    public string Code => GetType().Name;
    
    public string Message => "User was not found.";
}