using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Audios.ResetPrivateKey
{
    public record ResetAudioPrivateKeyRequest(long AudioId) : IRequest<IResult<string>>
    {
        
    }
}