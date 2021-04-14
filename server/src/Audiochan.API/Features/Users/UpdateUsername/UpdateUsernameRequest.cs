using System.Text.Json.Serialization;
using Audiochan.Core.Models.Interfaces;
using MediatR;

namespace Audiochan.API.Features.Users.UpdateUsername
{
    public record UpdateUsernameRequest : IRequest<IResult<bool>>
    {
        [JsonIgnore] public string UserId { get; init; }
        public string NewUsername { get; init; }
    }
}