using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Followers.GetFollowings
{
    public record GetUserFollowingsRequest : IHasPage, IRequest<PagedList<FollowingViewModel>>
    {
        public string? Username { get; init; }
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class GetUserFollowingsRequestHandler : IRequestHandler<GetUserFollowingsRequest, PagedList<FollowingViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly MediaStorageSettings _storageSettings;

        public GetUserFollowingsRequestHandler(IApplicationDbContext dbContext, IOptions<MediaStorageSettings> options)
        {
            _dbContext = dbContext;
            _storageSettings = options.Value;
        }

        public async Task<PagedList<FollowingViewModel>> Handle(GetUserFollowingsRequest request,
            CancellationToken cancellationToken)
        {
            // return await _dbContext.FollowedUsers
            //     .AsNoTracking()
            //     .Include(u => u.Target)
            //     .Include(u => u.Observer)
            //     .Where(u => request.Username != null && u.Observer.UserName == request.Username.Trim().ToLower())
            //     .ProjectToFollowing(_storageSettings)
            //     .PaginateAsync(request, cancellationToken);
            
            return await _dbContext.Users
                .Include(u => u.Followings)
                .Where(u => u.UserName == request.Username)
                .ProjectToFollowing(_storageSettings)
                .PaginateAsync(request, cancellationToken);
        }
    }
}