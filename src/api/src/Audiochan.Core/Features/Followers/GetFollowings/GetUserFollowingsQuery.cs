using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Followers.GetFollowings
{
    public record GetUserFollowingsQuery : IHasOffsetPage, IRequest<OffsetPagedListDto<FollowingViewModel>>
    {
        public string Username { get; init; } = string.Empty;
        public int Offset { get; init; }
        public int Size { get; init; }
    }
    
    public sealed class GetUserFollowingsSpecification : Specification<FollowedUser, FollowingViewModel>
    {
        public GetUserFollowingsSpecification(string username)
        {
            Query.AsNoTracking();
            Query.Where(u => u.Observer.UserName == username);
            Query.OrderByDescending(u => u.FollowedDate);
            Query.Select(FollowedUserMaps.UserToFollowingFunc);
        }
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
            var list = await _unitOfWork.FollowedUsers
                .GetOffsetPagedListAsync(new GetUserFollowingsSpecification(query.Username), query.Offset, query.Size, cancellationToken);
            return new OffsetPagedListDto<FollowingViewModel>(list, query.Offset, query.Size);
        }
    }
}