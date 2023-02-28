using System;
using Audiochan.Common.Models;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Domain.Entities;
using Audiochan.Domain.Exceptions;
using HotChocolate.Types.Relay;

namespace Audiochan.API.Features.Audios.Errors;

public class AudioNotFoundError : IUserError
{
    public AudioNotFoundError(long id, string? message = null)
    {
        Id = id;
        Message = message ?? $"Audio with id {id} was not found.";
    }
    
    [ID(nameof(AudioDto))]
    public long Id { get; }
    public string Code => GetType().Name;
    public string Message { get; }
}