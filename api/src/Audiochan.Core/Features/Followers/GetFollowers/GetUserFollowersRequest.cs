using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Interfaces;
using MediatR;

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
            return await _unitOfWork.FollowedUsers.GetPagedListBySpec(new GetUserFollowersSpecification(request.Username),
                request.Page, request.Size, cancellationToken: cancellationToken);
        }
    }
}