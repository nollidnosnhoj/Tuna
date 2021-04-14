using System.Text.Json.Serialization;
using Audiochan.Core.Models.Interfaces;
using Audiochan.Core.Models.Requests;
using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.API.Features.Audios.UpdateAudio
{
    public class UpdateAudioRequest : AudioAbstractRequest, IRequest<IResult<AudioDetailViewModel>>
    {
        [JsonIgnore] public long AudioId { get; set; }
    }
}