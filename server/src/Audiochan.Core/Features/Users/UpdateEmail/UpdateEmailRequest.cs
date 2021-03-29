using System.Text.Json.Serialization;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Users.UpdateEmail
{
    public record UpdateEmailRequest : IRequest<IResult<bool>>
    {
        [JsonIgnore] public string UserId { get; init; }
        public string NewEmail { get; init; }
    }
}