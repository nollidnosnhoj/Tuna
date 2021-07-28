using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetLatestAudios;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.GetAudioFeed
{
    public record GetAudioFeedQuery : IHasPage, IRequest<PagedListDto<AudioViewModel>>
    {
        public string UserId { get; init; } = string.Empty;
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class GetAudioFeedQueryHandler : IRequestHandler<GetAudioFeedQuery, PagedListDto<AudioViewModel>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetAudioFeedQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedListDto<AudioViewModel>> Handle(GetAudioFeedQuery query,
            CancellationToken cancellationToken)
        {
            var followingIds = await _dbContext.Users
                .Include(u => u.Followings)
                .AsNoTracking()
                .AsSplitQuery()
                .Where(user => user.Id == query.UserId)
                .SelectMany(u => u.Followings.Select(f => f.TargetId))
                .ToListAsync(cancellationToken);
            
            return await _dbContext.Audios
                .AsNoTracking()
                .Include(x => x.Tags)
                .Include(x => x.User)
                .Where(a => a.Visibility == Visibility.Public)
                .Where(a => followingIds.Contains(a.UserId))
                .Select(AudioMaps.AudioToView)
                .OrderByDescending(a => a.Created)
                .PaginateAsync(cancellationToken: cancellationToken);
        }
    }
}