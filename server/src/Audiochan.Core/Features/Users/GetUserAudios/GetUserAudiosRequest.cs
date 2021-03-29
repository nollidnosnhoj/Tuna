using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios.GetAudioList;
using MediatR;

namespace Audiochan.Core.Features.Users.GetUserAudios
{
    public record GetUserAudiosRequest : AudioListQueryRequest, IRequest<PagedList<AudioViewModel>>
    {
        public string Username { get; init; }
    }
}