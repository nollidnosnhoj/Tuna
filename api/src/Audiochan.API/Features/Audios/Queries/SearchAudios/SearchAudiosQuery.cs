using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.CQRS;
using Audiochan.Core.Dtos;
using Audiochan.Core.Dtos.Filters;
using Audiochan.Core.Dtos.Wrappers;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Audios.Queries
{
    public record SearchAudiosQuery : IHasPage, IQueryRequest<PagedListDto<AudioDto>>
    {
        public string Q { get; init; } = string.Empty;
        public string Tags { get; init; } = string.Empty;
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class SearchAudiosQueryHandler : IRequestHandler<SearchAudiosQuery, PagedListDto<AudioDto>>
    {
        private readonly ISearchService _searchService;

        public SearchAudiosQueryHandler(ISearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<PagedListDto<AudioDto>> Handle(SearchAudiosQuery query,
            CancellationToken cancellationToken)
        {
            return await _searchService
                .SearchAudiosAsync(new SearchAudioFilter(query.Q, query.Tags, query.Page, query.Size), cancellationToken);
        }
    }
}