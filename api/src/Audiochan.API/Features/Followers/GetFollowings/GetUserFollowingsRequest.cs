using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Mappings;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Persistence.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.API.Features.Followers.GetFollowings
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
            return await _unitOfWork.FollowedUsers
                .AsNoTracking()
                .Include(u => u.Target)
                .Include(u => u.Observer)
                .Where(u => u.Observer.UserName == request.Username.Trim().ToLower())
                .OrderByDescending(x => x.FollowedDate)
                .ProjectToFollowing()
                .PaginateAsync(request, cancellationToken);
        }
    }
}