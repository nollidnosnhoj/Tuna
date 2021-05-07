using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Extensions.QueryableExtensions;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;

namespace Audiochan.Core.Features.Audios.GetAudioList
{
    public record GetAudioListRequest : IHasCursor, IRequest<CursorList<AudioViewModel>>
    {
        public string Cursor { get; init; }
        public int Size { get; init; } = 30;
    }

    public class GetAudioListRequestHandler : IRequestHandler<GetAudioListRequest, CursorList<AudioViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly MediaStorageSettings _storageSettings;

        public GetAudioListRequestHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService,
            IOptions<MediaStorageSettings> options)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _storageSettings = options.Value;
        }

        public async Task<CursorList<AudioViewModel>> Handle(GetAudioListRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var (dateTime, id) = CursorHelpers.DecodeCursor(request.Cursor);

            var queryable = _dbContext.Audios
                .DefaultQueryable()
                .ExcludePrivateAudios();

            if (dateTime.HasValue && !string.IsNullOrWhiteSpace(id))
            {
                queryable = queryable.Where(a => a.Created < dateTime.GetValueOrDefault()
                    || (a.Created == dateTime.GetValueOrDefault() && string.Compare(a.Id, id) < 0));
            }

            var audios = await queryable
                .OrderByDescending(a => a.Created)
                .ThenByDescending(a => a.Id)
                .ProjectToList(_storageSettings)
                .Take(request.Size)
                .ToListAsync(cancellationToken);

            var lastAudio = audios.LastOrDefault();

            var nextCursor = audios.Count < request.Size
                ? null
                : lastAudio != null
                    ? CursorHelpers.EncodeCursor(Instant.FromDateTimeUtc(lastAudio.Uploaded), lastAudio.Id)
                    : null;

            return new CursorList<AudioViewModel>(audios, nextCursor);
        }
    }
}