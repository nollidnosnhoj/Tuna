using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Audios.RemoveAudio
{
    public record RemoveAudioRequest(long Id) : IRequest<IResult<bool>>
    {
    }
}