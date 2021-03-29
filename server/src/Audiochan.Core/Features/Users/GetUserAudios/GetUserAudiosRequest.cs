using Audiochan.Core.Common.Models.Requests;

namespace Audiochan.Core.Features.Users.GetUserAudios
{
    public record GetUserAudiosRequest : AudioListQueryRequest
    {
        public string Username { get; init; }
    }
}