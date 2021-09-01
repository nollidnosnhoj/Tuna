using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.SearchAudios;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Search
{
    public class PostgresSearchService : ISearchService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ApplicationDbContext _dbContext;

        public PostgresSearchService(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<PagedListDto<AudioDto>> SearchAudiosAsync(SearchAudiosQuery query, CancellationToken ct = default)
        {
            var currentUserId = _currentUserService.GetUserId();
            
            var parsedTags = !string.IsNullOrWhiteSpace(query.Tags)
                ? query.Tags.Split(',')
                    .Select(t => t.Trim().ToLower())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToList()
                : new List<string>();

            var queryable = _dbContext.Audios
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Q))
                queryable = queryable.Where(a => 
                    EF.Functions.Like(a.Title.ToLower(), $"%{query.Q.ToLower()}%"));

            if (parsedTags.Count > 0)
                queryable = queryable.Where(a => a.Tags.Any(x => parsedTags.Contains(x.Name)));

            return await queryable
                .Select(AudioMaps.AudioToView(currentUserId))
                .PaginateAsync(query, ct);
        }
    }
}