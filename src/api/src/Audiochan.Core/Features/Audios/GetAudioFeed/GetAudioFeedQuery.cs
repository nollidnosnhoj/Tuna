using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.GetAudioFeed
{
    public record GetAudioFeedQuery : IHasOffsetPage, IRequest<OffsetPagedListDto<AudioDto>>
    {
        public long UserId { get; init; }
        public int Offset { get; init; }
        public int Size { get; init; }
    }

    public class GetAudioFeedQueryHandler : IRequestHandler<GetAudioFeedQuery, OffsetPagedListDto<AudioDto>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly long _currentUserId;

        public GetAudioFeedQueryHandler(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<OffsetPagedListDto<AudioDto>> Handle(GetAudioFeedQuery query,
            CancellationToken cancellationToken)
        {
            var followingIds = await _dbContext.Users
                .Include(u => u.Followings)
                .AsNoTracking()
                .Where(user => user.Id == query.UserId)
                .SelectMany(u => u.Followings.Select(f => f.TargetId))
                .ToListAsync(cancellationToken);
            
            return await _dbContext.Audios
                .AsNoTracking()
                .Where(a => followingIds.Contains(a.UserId))
                .Select(AudioMaps.AudioToView(_currentUserId))
                .OrderByDescending(a => a.Created)
                .OffsetPaginateAsync(cancellationToken: cancellationToken);
        }
    }
}