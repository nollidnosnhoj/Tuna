using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Users.GetFollowers
{
    public record GetUserFollowersQuery(string Username) : IHasOffsetPage, IRequest<OffsetPagedListDto<FollowerViewModel>>
    {
        public int Offset { get; init; }
        public int Size { get; init; }
    }

    public sealed class GetUserFollowersSpecification : Specification<FollowedUser>
    {
        public GetUserFollowersSpecification(string username)
        {
            Query.AsNoTracking();
            Query.Where(u => u.Target.UserName == username);
            Query.OrderByDescending(u => u.FollowedDate);
        }
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
                .GetOffsetPagedListAsync<FollowerViewModel>(new GetUserFollowersSpecification(query.Username), query.Offset, query.Size, cancellationToken);
            return new OffsetPagedListDto<FollowerViewModel>(list, query.Offset, query.Size);
        }
    }
}