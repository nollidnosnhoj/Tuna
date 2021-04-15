using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.Features.Audios.GetAudio
{
    public record GetAudioRequest(long Id, string PrivateKey = "") : IRequest<AudioDetailViewModel>
    {
    }
}