using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.API.Features.Audios.GetRandomAudio
{
    public record GetRandomAudioRequest : IRequest<AudioDetailViewModel>
    {
    }
}