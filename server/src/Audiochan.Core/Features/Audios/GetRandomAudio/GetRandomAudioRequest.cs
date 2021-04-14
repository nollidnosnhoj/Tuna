using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.Core.Features.Audios.GetRandomAudio
{
    public record GetRandomAudioRequest : IRequest<AudioDetailViewModel>
    {
    }
}