using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.GetAudios
{
    public record GetAudiosQuery : IHasCursorPage<long>, IRequest<CursorPagedListDto<AudioDto>>
    {
        public List<string> Tags { get; init; } = new();
        public long? Cursor { get; init; }
        public int Size { get; init; } = 30;
    }

    public class GetAudiosQueryHandler : IRequestHandler<GetAudiosQuery, CursorPagedListDto<AudioDto>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly long _currentUserId;

        public GetAudiosQueryHandler(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<CursorPagedListDto<AudioDto>> Handle(GetAudiosQuery query,
            CancellationToken cancellationToken)
        {
            var queryable = _dbContext.Audios
                .AsNoTracking();


            if (query.Tags.Count > 0)
            {
                queryable = queryable
                    .Where(a => a.Tags.Any(t => query.Tags.Contains(t.Name)));
            }

            return await queryable
                .OrderByDescending(a => a.Id)
                .Select(AudioMaps.AudioToView(_currentUserId))
                .CursorPaginateAsync(query, cancellationToken);
        }
    }
}