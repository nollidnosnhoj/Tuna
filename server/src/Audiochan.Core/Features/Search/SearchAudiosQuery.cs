using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Search
{
    public record SearchAudiosQuery : AudioListQueryRequest
    {
        public string Q { get; init; }
        public string Tags { get; init; }
    }

    public class SearchAudiosQueryHandler : IRequestHandler<SearchAudiosQuery, PagedList<AudioViewModel>>
    {
        private readonly ISearchService _searchService;

        public SearchAudiosQueryHandler(ISearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<PagedList<AudioViewModel>> Handle(SearchAudiosQuery request,
            CancellationToken cancellationToken)
        {
            return await _searchService.SearchAudios(request, cancellationToken);
        }
    }
}