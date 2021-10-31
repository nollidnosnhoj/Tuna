using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models.Pagination;
using MediatR;

namespace Audiochan.Core.Audios.Queries
{
    public record SearchAudiosQuery : IHasPage, IRequest<PagedListDto<AudioDto>>
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
            return await _searchService.SearchAudiosAsync(query, cancellationToken);
        }
    }
}