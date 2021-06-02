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

namespace Audiochan.API.Features.Followers.GetFollowers
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