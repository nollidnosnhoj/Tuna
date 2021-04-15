using System.Text.Json.Serialization;
using Audiochan.Core.Models.Interfaces;
using MediatR;

namespace Audiochan.Features.Users.UpdateUser
{
    public record UpdateUserDetailsRequest : IRequest<IResult<bool>>
    {
        [JsonIgnore] public string UserId { get; init; }
        public string DisplayName { get; init; }
        public string About { get; init; }
        public string Website { get; init; }
    }
}