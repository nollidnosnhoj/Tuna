using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Followers.GetFollowings
{
    public record GetUserFollowingsRequest : IHasPage, IRequest<PagedListDto<FollowingViewModel>>
    {
        public string Username { get; init; } = string.Empty;
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class GetUserFollowingsRequestHandler : IRequestHandler<GetUserFollowingsRequest, PagedListDto<FollowingViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserFollowingsRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedListDto<FollowingViewModel>> Handle(GetUserFollowingsRequest request,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.FollowedUsers.GetPagedListBySpec(
                new GetUserFollowingsSpecification(request.Username), request.Page, request.Size, cancellationToken: cancellationToken);
        }
    }
}