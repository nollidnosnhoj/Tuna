using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.Features.Audios.GetRandomAudio
{
    public record GetRandomAudioRequest : IRequest<AudioDetailViewModel>
    {
    }
}