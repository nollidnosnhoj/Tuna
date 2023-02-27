using System.Collections.Generic;
using Audiochan.Common.Models;

namespace Audiochan.API.Features.Audios.Payloads;

public class RemoveAudioPayload : Payload<UserError>
{
    public RemoveAudioPayload()
    {
        
    }
    
    public RemoveAudioPayload(params UserError[] errors) : base(errors)
    {
    }

    public RemoveAudioPayload(IEnumerable<UserError> errors, string? message = null) : base(errors, message)
    {
    }

    public RemoveAudioPayload(string? message) : base(message)
    {
    }
}