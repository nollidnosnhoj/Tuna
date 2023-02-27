using System.Collections.Generic;
using Audiochan.Common.Models;

namespace Audiochan.API.Features.Audios.Payloads;

public class SetFavoriteAudioPayload : Payload<UserError>
{
    public SetFavoriteAudioPayload(bool isFavorited)
    {
        IsFavorited = isFavorited;
    }

    public SetFavoriteAudioPayload(params UserError[] errors) : base(errors)
    {
    }

    public SetFavoriteAudioPayload(IEnumerable<UserError> errors, string? message = null) : base(errors, message)
    {
    }

    public SetFavoriteAudioPayload(string? message) : base(message)
    {
    }
    
    public bool? IsFavorited { get; }
}