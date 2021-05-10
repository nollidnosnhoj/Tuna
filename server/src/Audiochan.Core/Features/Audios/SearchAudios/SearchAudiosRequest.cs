using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Audios.SearchAudios
{
    public record SearchAudiosRequest : IHasPage, IRequest<PagedList<AudioViewModel>>
    {
        public string Q { get; init; } = string.Empty;
        public string Tags { get; init; } = string.Empty;
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class SearchAudiosRequestHandler : IRequestHandler<SearchAudiosRequest, PagedList<AudioViewModel>>
    {
        private readonly ISearchService _searchService;

        public SearchAudiosRequestHandler(ISearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<PagedList<AudioViewModel>> Handle(SearchAudiosRequest request,
            CancellationToken cancellationToken)
        {
            return await _searchService.SearchAudios(request, cancellationToken);
        }
    }
}