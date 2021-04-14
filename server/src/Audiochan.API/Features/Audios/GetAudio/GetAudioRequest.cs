using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.API.Features.Audios.GetAudio
{
    public record GetAudioRequest(long Id, string PrivateKey = "") : IRequest<AudioDetailViewModel>
    {
    }
}