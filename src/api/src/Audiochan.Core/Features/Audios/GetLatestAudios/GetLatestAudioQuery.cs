using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.GetLatestAudios
{
    public record GetLatestAudioQuery : IHasCursorPage<long>, IRequest<CursorPagedListDto<AudioViewModel>>
    {
        public long? Cursor { get; init; }
        public int Size { get; init; } = 30;
    }

    public class GetLatestAudioQueryHandler : IRequestHandler<GetLatestAudioQuery, CursorPagedListDto<AudioViewModel>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly long _currentUserId;

        public GetLatestAudioQueryHandler(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<CursorPagedListDto<AudioViewModel>> Handle(GetLatestAudioQuery query,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Audios
                .AsNoTracking()
                .Include(x => x.Tags)
                .Include(x => x.User)
                .Where(a => a.UserId == _currentUserId || a.Visibility == Visibility.Public)
                .Select(AudioMaps.AudioToView)
                .CursorPaginateAsync(query, cancellationToken);
        }
    }
}