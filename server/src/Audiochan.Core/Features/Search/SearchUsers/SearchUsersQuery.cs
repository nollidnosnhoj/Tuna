using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Users.GetUser;
using Audiochan.Core.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Search.SearchUsers
{
    public record SearchUsersQuery : PaginationQueryRequest<UserViewModel>
    {
        public string Q { get; init; }
    }

    public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, PagedList<UserViewModel>>
    {
        private readonly ISearchService _searchService;

        public SearchUsersQueryHandler(ISearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<PagedList<UserViewModel>> Handle(SearchUsersQuery request,
            CancellationToken cancellationToken)
        {
            return await _searchService.SearchUsers(request, cancellationToken);
        }
    }
}