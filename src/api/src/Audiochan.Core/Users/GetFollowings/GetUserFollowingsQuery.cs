using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Models.Pagination;
using MediatR;

namespace Audiochan.Core.Users
{
    public record GetUserFollowingsQuery : IHasOffsetPage, IRequest<OffsetPagedListDto<FollowingViewModel>>
    {
        public string Username { get; init; } = string.Empty;
        public int Offset { get; init; }
        public int Size { get; init; }
    }

    public class GetUserFollowingsQueryHandler : IRequestHandler<GetUserFollowingsQuery, OffsetPagedListDto<FollowingViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserFollowingsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OffsetPagedListDto<FollowingViewModel>> Handle(GetUserFollowingsQuery query,
            CancellationToken cancellationToken)
        {
            var spec = new GetFollowerByObserverName(query.Username);
            var list = await _unitOfWork.FollowedUsers
                .GetOffsetPagedListAsync<FollowingViewModel>(spec, query.Offset, query.Size, cancellationToken);
            return new OffsetPagedListDto<FollowingViewModel>(list, query.Offset, query.Size);
        }
    }
}