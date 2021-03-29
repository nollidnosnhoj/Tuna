using System.Text.Json.Serialization;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Users.UpdatePassword
{
    public record UpdatePasswordRequest : IRequest<IResult<bool>>
    {
        [JsonIgnore] public string UserId { get; init; }
        public string CurrentPassword { get; init; } = "";
        public string NewPassword { get; init; } = "";
    }
}