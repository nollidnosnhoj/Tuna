using Audiochan.Core.Models.Interfaces;
using MediatR;

namespace Audiochan.API.Features.Audios.ResetPrivateKey
{
    public record ResetAudioPrivateKeyRequest(long AudioId) : IRequest<IResult<string>>
    {
        
    }
}