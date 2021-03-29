using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Features.Users.GetUser;

namespace Audiochan.Core.Features.Search.SearchUsers
{
    public record SearchUsersRequest : PaginationQueryRequest<UserViewModel>
    {
        public string Q { get; init; }
    }
}