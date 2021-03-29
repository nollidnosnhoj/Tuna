using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Audios.GetAudioList
{
    public record GetAudioListRequest : AudioListQueryRequest, IRequest<PagedList<AudioViewModel>>
    {
    }
}