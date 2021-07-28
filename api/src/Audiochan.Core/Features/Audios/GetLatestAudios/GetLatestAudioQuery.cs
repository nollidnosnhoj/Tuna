using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.GetAudio;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.GetLatestAudios
{
    public record GetLatestAudioQuery : IRequest<GetAudioListViewModel>
    {
        public string? Tag { get; init; }
        public string? Cursor { get; init; }
        public int Size { get; init; } = 30;
    }

    public class GetLatestAudioQueryHandler : IRequestHandler<GetLatestAudioQuery, GetAudioListViewModel>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetLatestAudioQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetAudioListViewModel> Handle(GetLatestAudioQuery query,
            CancellationToken cancellationToken)
        {
            var audios = await _dbContext.Audios
                .AsNoTracking()
                .Include(x => x.Tags)
                .Include(x => x.User)
                .Where(a => a.Visibility == Visibility.Public)
                .FilterCursor(query.Cursor)
                .Select(AudioMaps.AudioToView)
                .Take(query.Size)
                .ToListAsync(cancellationToken);

            var nextCursor = GetNextCursor(audios, query.Size);

            return new GetAudioListViewModel(audios, nextCursor);
        }

        private string? GetNextCursor(List<AudioViewModel> audios, int size)
        {
            var lastAudio = audios.LastOrDefault();

            return audios.Count < size
                ? null
                : lastAudio != null
                    ? CursorHelpers.Encode(lastAudio.Id, lastAudio.Created)
                    : null;
        }
    }
}