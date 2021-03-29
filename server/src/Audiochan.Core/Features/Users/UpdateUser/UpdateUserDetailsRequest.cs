using System.Text.Json.Serialization;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Users.UpdateUser
{
    public record UpdateUserDetailsRequest : IRequest<IResult<bool>>
    {
        [JsonIgnore] public string UserId { get; init; }
        public string DisplayName { get; init; }
        public string About { get; init; }
        public string Website { get; init; }
    }
}