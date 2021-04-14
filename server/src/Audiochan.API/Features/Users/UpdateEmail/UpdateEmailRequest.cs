using System.Text.Json.Serialization;
using Audiochan.Core.Models.Interfaces;
using MediatR;

namespace Audiochan.API.Features.Users.UpdateEmail
{
    public record UpdateEmailRequest : IRequest<IResult<bool>>
    {
        [JsonIgnore] public string UserId { get; init; }
        public string NewEmail { get; init; }
    }
}