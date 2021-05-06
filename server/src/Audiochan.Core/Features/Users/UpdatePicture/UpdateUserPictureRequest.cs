using System.Text.Json.Serialization;
using Audiochan.Core.Common.Models.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Users.UpdatePicture
{
    public record UpdateUserPictureRequest : IRequest<IResult<string>>
    {
        [JsonIgnore] public string UserId { get; set; }
        public string Data { get; init; }
    }
}