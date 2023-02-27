using System.Collections.Generic;
using Audiochan.Common.Models;
using Audiochan.Core.Features.Audios.Models;

namespace Audiochan.API.Features.Audios.Payloads;

public class AudioPayload : Payload<UserError>
{
    public AudioPayload(AudioViewModel audio)
    {
        Audio = audio;
    }
    
    public AudioPayload(params UserError[] errors) : base(errors)
    {
    }

    public AudioPayload(IEnumerable<UserError> errors, string? message = null) : base(errors, message)
    {
    }

    public AudioPayload(string? message) : base(message)
    {
    }
    
    public AudioViewModel? Audio { get; }
}