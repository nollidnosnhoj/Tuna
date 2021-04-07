using System.Text.Json.Serialization;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios.GetAudio;
using MediatR;

namespace Audiochan.Core.Features.Audios.UpdateAudio
{
    public record UpdateAudioRequest : AudioAbstractRequest, IRequest<IResult<AudioDetailViewModel>>
    {
        [JsonIgnore] public long Id { get; init; }
    }
}