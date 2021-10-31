using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Models.Pagination;
using MediatR;

namespace Audiochan.Core.Users
{
    public record GetUserFollowersQuery(string Username) : IHasOffsetPage, IRequest<OffsetPagedListDto<FollowerViewModel>>
    {
        public int Offset { get; init; }
        public int Size { get; init; }
    }

    public class GetUserFollowersQueryHandler : IRequestHandler<GetUserFollowersQuery, OffsetPagedListDto<FollowerViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserFollowersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OffsetPagedListDto<FollowerViewModel>> Handle(GetUserFollowersQuery query,
            CancellationToken cancellationToken)
        {
            var list = await _unitOfWork.FollowedUsers
                .GetOffsetPagedListAsync<FollowerViewModel>(new GetFollowerByTargetNameSpecification(query.Username), query.Offset, query.Size, cancellationToken);
            return new OffsetPagedListDto<FollowerViewModel>(list, query.Offset, query.Size);
        }
    }
}