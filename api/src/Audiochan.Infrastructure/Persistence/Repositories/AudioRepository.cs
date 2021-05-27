using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Interfaces;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    internal class AudioRepository : EfRepository<Audio>, IAudioRepository
    {
        public AudioRepository([NotNull] ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public AudioRepository([NotNull] ApplicationDbContext dbContext,
            [NotNull] ISpecificationEvaluator specificationEvaluator)
            : base(dbContext, specificationEvaluator)
        {
        }

        public async Task<CursorListDto<AudioViewModel>> GetCursorPaginationAsync(
            ISpecification<Audio, AudioViewModel> specification,
            string? cursor, int size = 30, CancellationToken cancellationToken = default)
        {
            var queryable = ApplySpecification(specification);
            
            if (!string.IsNullOrEmpty(cursor))
            {
                var (since, id) = CursorHelpers.DecodeCursor(cursor);
                if (Guid.TryParse(id, out var audioId) && since.HasValue)
                {
                    queryable = queryable
                        .Where(a => a.Uploaded < since.GetValueOrDefault() 
                                    || a.Uploaded == since.GetValueOrDefault() && a.Id.CompareTo(audioId) < 0);
                }
            }

            var list = await queryable
                .OrderByDescending(a => a.Uploaded)
                .ThenByDescending(a => a.Id)
                .Take(size)
                .ToListAsync(cancellationToken);


            var lastAudio = list.LastOrDefault();

            var nextCursor = list.Count < specification.Take
                ? null
                : lastAudio != null
                    ? CursorHelpers.EncodeCursor(lastAudio.Uploaded, lastAudio.Id.ToString())
                    : null;

            return new CursorListDto<AudioViewModel>(list, nextCursor);
        }
    }
}