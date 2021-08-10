using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.GetLatestAudios
{
    public record GetLatestAudioQuery : IRequest<GetAudioListViewModel>
    {
        public string? Tag { get; init; }
        public long? Cursor { get; init; }
        public int Size { get; init; } = 30;
    }

    public class GetLatestAudioQueryHandler : IRequestHandler<GetLatestAudioQuery, GetAudioListViewModel>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly long _currentUserId;

        public GetLatestAudioQueryHandler(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<GetAudioListViewModel> Handle(GetLatestAudioQuery query,
            CancellationToken cancellationToken)
        {
            var audios = await _dbContext.Audios
                .AsNoTracking()
                .Include(x => x.Tags)
                .Include(x => x.User)
                .FilterVisibility(_currentUserId, FilterVisibilityMode.OnlyPublic)
                .FilterCursor(query.Cursor)
                .Select(AudioMaps.AudioToView)
                .Take(query.Size)
                .ToListAsync(cancellationToken);

            var nextCursor = GetNextCursor(audios, query.Size);

            return new GetAudioListViewModel(audios, nextCursor);
        }

        private long? GetNextCursor(List<AudioViewModel> audios, int size)
        {
            var lastAudio = audios.LastOrDefault();

            return audios.Count < size
                ? null
                : lastAudio?.Id;
        }
    }
}