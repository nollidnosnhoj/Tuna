using MediatR;

namespace Audiochan.Core.Features.Audios.GetAudio
{
    public record GetAudioRequest(long Id, string PrivateKey = "") : IRequest<AudioDetailViewModel>
    {
    }
}