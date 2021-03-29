using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Audios.GetAudio
{
    public record GetAudioRequest(long Id, string PrivateKey = "") : IRequest<Result<AudioDetailViewModel>>
    {
    }
}