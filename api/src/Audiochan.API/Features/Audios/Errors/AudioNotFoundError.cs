using Audiochan.Shared.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Exceptions;
using Audiochan.Core.Features.Audios.Models;
using HotChocolate.Types.Relay;

namespace Audiochan.API.Features.Audios.Errors;

public class AudioNotFoundError : IUserError
{
    public AudioNotFoundError(EntityNotFoundException<Audio, long> ex)
    {
        Id = ex.Id;
        Code = GetType().Name;
        Message = ex.Message;
    }
    
    [ID(nameof(AudioDto))]
    public long Id { get; }
    public string Code { get; }
    public string Message { get; }
}