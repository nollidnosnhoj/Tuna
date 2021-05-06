using System.Text.Json.Serialization;
using Audiochan.Core.Common.Models.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Audios.UpdatePicture
{
    public class UpdateAudioPictureRequest : IRequest<IResult<string>>
    {
        [JsonIgnore] public long AudioId { get; set; }
        public string Data { get; init; }
    }
}