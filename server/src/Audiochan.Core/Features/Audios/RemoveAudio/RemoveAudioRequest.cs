using Audiochan.Core.Models.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Audios.RemoveAudio
{
    public record RemoveAudioRequest(long Id) : IRequest<IResult<bool>>
    {
    }
}