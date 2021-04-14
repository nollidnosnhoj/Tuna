using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models.Responses;
using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.API.Features.Search.SearchAudios
{
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
            var parsedTags = request.Tags.Split(',')
                .Select(t => t.Trim().ToLower())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToArray();
            
            return await _searchService.SearchAudios(request.Q, parsedTags, request.Page, request.Size, cancellationToken);
        }
    }
}