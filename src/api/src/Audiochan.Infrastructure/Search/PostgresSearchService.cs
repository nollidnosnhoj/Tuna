using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Commons.Dtos.Wrappers;
using Audiochan.Core.Commons.Extensions;

using Audiochan.Core.Commons.Services;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Persistence;
using Audiochan.Domain.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Search
{
    public class PostgresSearchService : ISearchService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public PostgresSearchService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PagedListDto<AudioDto>> SearchAudiosAsync(SearchAudioFilter filter, CancellationToken ct = default)
        {
            var count = await GetQueryable(filter).CountAsync(ct);
            var results = await GetQueryable(filter)
                .ProjectTo<AudioDto>(_mapper.ConfigurationProvider)
                .PaginateAsync(filter.Page, filter.Size, ct);
            return new PagedListDto<AudioDto>(results, count, filter.Page, filter.Size);
        }

        private IQueryable<Audio> GetQueryable(SearchAudioFilter query)
        {
            var parsedTags = !string.IsNullOrWhiteSpace(query.Tags)
                ? query.Tags.Split(',')
                    .Select(t => t.Trim().ToLower())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToList()
                : new List<string>();

            var queryable = _dbContext.Audios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Term))
                queryable = queryable.Where(a => 
                    EF.Functions.Like(a.Title.ToLower(), $"%{query.Term.ToLower()}%"));

            if (parsedTags.Count > 0)
                queryable = queryable.Where(a => a.Tags.Any(x => parsedTags.Contains(x)));

            return queryable;
        }
    }
}