using System.Text.Json.Serialization;
using Audiochan.Core.Models.Interfaces;
using Audiochan.Core.Models.Requests;
using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.Core.Features.Audios.UpdateAudio
{
    public record UpdateAudioRequest : AudioAbstractRequest, IRequest<IResult<AudioDetailViewModel>>
    {
        [JsonIgnore] public long Id { get; init; }
    }
}