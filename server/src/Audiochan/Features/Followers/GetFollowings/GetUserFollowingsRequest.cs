using Audiochan.Core.Models.Interfaces;
using Audiochan.Core.Models.Responses;
using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.Features.Followers.GetFollowings
{
    public record GetUserFollowingsRequest : IHasPage, IRequest<PagedList<FollowingViewModel>>
    {
        public string Username { get; init; }
        public int Page { get; init; }
        public int Size { get; init; }
    }
}