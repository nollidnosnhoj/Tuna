using Audiochan.Core.Models.Interfaces;
using Audiochan.Core.Models.Responses;
using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.API.Features.Followers.GetFollowers
{
    public record GetUserFollowersRequest : IHasPage, IRequest<PagedList<FollowerViewModel>>
    {
        public string Username { get; init; }
        public int Page { get; init; }
        public int Size { get; init; }
    }
}