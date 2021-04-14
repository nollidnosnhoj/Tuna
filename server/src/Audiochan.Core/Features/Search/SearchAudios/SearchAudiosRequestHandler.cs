using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Models.ViewModels;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Search.SearchAudios
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
            return await _searchService.SearchAudios(request, cancellationToken);
        }
    }
}