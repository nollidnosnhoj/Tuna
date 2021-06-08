using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Followers.GetFollowings
{
    public record GetUserFollowingsQuery : IHasPage, IRequest<PagedListDto<FollowingViewModel>>
    {
        public string Username { get; init; } = string.Empty;
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class GetUserFollowingsQueryHandler : IRequestHandler<GetUserFollowingsQuery, PagedListDto<FollowingViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserFollowingsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedListDto<FollowingViewModel>> Handle(GetUserFollowingsQuery query,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.FollowedUsers
                .AsNoTracking()
                .Include(u => u.Target)
                .Include(u => u.Observer)
                .Where(u => u.Observer.UserName == query.Username.Trim().ToLower())
                .OrderByDescending(x => x.FollowedDate)
                .ProjectToFollowing()
                .PaginateAsync(query, cancellationToken);
        }
    }
}