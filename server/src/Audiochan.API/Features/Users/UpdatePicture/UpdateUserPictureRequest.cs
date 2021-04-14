using System.Text.Json.Serialization;
using Audiochan.Core.Models.Interfaces;
using MediatR;

namespace Audiochan.API.Features.Users.UpdatePicture
{
    public record UpdateUserPictureRequest : IRequest<IResult<string>>
    {
        [JsonIgnore] public string UserId { get; set; }
        public string Data { get; init; }
    }
}