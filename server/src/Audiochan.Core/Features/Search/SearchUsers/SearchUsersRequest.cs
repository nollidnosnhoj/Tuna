using Audiochan.Core.Models.Interfaces;
using Audiochan.Core.Models.Responses;
using Audiochan.Core.Models.ViewModels;
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