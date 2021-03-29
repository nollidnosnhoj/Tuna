using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios.GetAudioList;
using MediatR;

namespace Audiochan.Core.Features.Search.SearchAudios
{
    public record SearchAudiosRequest : AudioListQueryRequest, IRequest<PagedList<AudioViewModel>>
    {
        public string Q { get; init; } = string.Empty;
        public string Tags { get; init; } = string.Empty;
    }
}