using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Followers.GetFollowers
{
    public record GetUserFollowersRequest : IHasPage, IRequest<PagedListDto<FollowerViewModel>>
    {
        public string Username { get; init; } = string.Empty;
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class GetUserFollowersRequestHandler : IRequestHandler<GetUserFollowersRequest, PagedListDto<FollowerViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserFollowersRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedListDto<FollowerViewModel>> Handle(GetUserFollowersRequest request,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.FollowedUsers
                .AsNoTracking()
                .Include(u => u.Target)
                .Include(u => u.Observer)
                .Where(u => u.Target.UserName == request.Username.Trim().ToLower())
                .OrderByDescending(x => x.FollowedDate)
                .ProjectToFollower()
                .PaginateAsync(request, cancellationToken);
        }
    }
}