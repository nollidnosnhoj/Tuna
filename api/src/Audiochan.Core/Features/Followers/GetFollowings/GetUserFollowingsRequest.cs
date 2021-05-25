using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Followers.GetFollowings
{
    public record GetUserFollowingsRequest : IHasPage, IRequest<PagedList<FollowingViewModel>>
    {
        public string Username { get; init; } = string.Empty;
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class GetUserFollowingsRequestHandler : IRequestHandler<GetUserFollowingsRequest, PagedList<FollowingViewModel>>
    {
        private readonly IFollowedUserRepository _followedUserRepository;

        public GetUserFollowingsRequestHandler(IFollowedUserRepository followedUserRepository)
        {
            _followedUserRepository = followedUserRepository;
        }

        public async Task<PagedList<FollowingViewModel>> Handle(GetUserFollowingsRequest request,
            CancellationToken cancellationToken)
        {
            return await _followedUserRepository.GetPagedListBySpec(
                new GetUserFollowingsSpecification(request.Username), request.Page, request.Size, cancellationToken: cancellationToken);
        }
    }
}