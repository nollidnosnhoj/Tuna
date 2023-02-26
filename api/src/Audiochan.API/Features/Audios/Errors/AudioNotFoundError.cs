using System;
using Audiochan.Common.Exceptions;
using Audiochan.Common.Models;
using Audiochan.Core.Entities;
using HotChocolate.Types.Relay;
using HotChocolate.Utilities;

namespace Audiochan.API.Features.Audios.Errors;

public class AudioNotFoundError : IUserError
{
    public AudioNotFoundError(long id)
    {
        AudioId = id;
    }

    public static AudioNotFoundError? CreateErrorFrom(Exception exception)
    {
        if (exception is ResourceIdInvalidException<long> invalidEx 
            && invalidEx.Resource.EqualsOrdinal(nameof(Audio)))
        {
            return new AudioNotFoundError(invalidEx.Id);
        }

        if (exception is ResourceOwnershipException<long> forbidEx
            && forbidEx.Resource.EqualsOrdinal(nameof(Audio)))
        {
            return new AudioNotFoundError(forbidEx.Id);
        }

        return null;
    }

    [ID(nameof(Audio))]
    public long AudioId { get; }
    
    public string Code => GetType().Name;
    
    public string Message => "Audio was not found.";
}