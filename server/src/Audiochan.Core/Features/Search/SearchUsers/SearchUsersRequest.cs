using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Users.GetUser;
using MediatR;

namespace Audiochan.Core.Features.Search.SearchUsers
{
    public record SearchUsersRequest : IHasPage, IRequest<PagedList<UserViewModel>>
    {
        public string Q { get; init; }
        public int Page { get; init; }
        public int Size { get; init; }
    }
}