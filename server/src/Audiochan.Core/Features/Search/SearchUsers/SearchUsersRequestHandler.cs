using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Models.ViewModels;
using Audiochan.Core.Features.Users.GetUser;
using Audiochan.Core.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Search.SearchUsers
{
    public class SearchUsersRequestHandler : IRequestHandler<SearchUsersRequest, PagedList<UserViewModel>>
    {
        private readonly ISearchService _searchService;

        public SearchUsersRequestHandler(ISearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<PagedList<UserViewModel>> Handle(SearchUsersRequest request,
            CancellationToken cancellationToken)
        {
            return await _searchService.SearchUsers(request, cancellationToken);
        }
    }
}