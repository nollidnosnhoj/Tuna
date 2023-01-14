using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Common.Dtos;
using Audiochan.Common.Extensions;
using Audiochan.Common.Interfaces;
using Audiochan.Core.Features.Audios.Dtos;
using Audiochan.Core.Features.Audios.Mappings;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MR.EntityFrameworkCore.KeysetPagination;

namespace Audiochan.Core.Features.Audios.Queries.GetAudios
{
    public record GetAudiosQuery : IHasCursorPage<long>, IQueryRequest<CursorPagedListDto<AudioDto, long>>
    {
        public List<string> Tags { get; init; } = new();
        public long Cursor { get; init; }
        public int Size { get; init; } = 30;
    }

    public class GetAudiosQueryHandler : IRequestHandler<GetAudiosQuery, CursorPagedListDto<AudioDto, long>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetAudiosQueryHandler(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<CursorPagedListDto<AudioDto, long>> Handle(GetAudiosQuery query,
            CancellationToken cancellationToken)
        {
            _currentUserService.User.TryGetUserId(out var currentUserId);
            var queryable = _dbContext.Audios.AsNoTracking();

            if (query.Tags.Count > 0)
                queryable = queryable.Where(a => a.Tags.Any(t => query.Tags.Contains(t)));

            queryable = queryable.OrderByDescending(a => a.Id);

            // var reference = await _dbContext.Audios.FirstOrDefaultAsync(x => x.Id == query.Cursor, cancellationToken); 
            var keysetContext = queryable.KeysetPaginate(
                b => b.Ascending(a => a.Id), 
                KeysetPaginationDirection.Forward,
                new { Id = query.Cursor });

            var list = await keysetContext.Query
                .Project(currentUserId)
                .Take(query.Size)
                .ToListAsync(cancellationToken);
            
            keysetContext.EnsureCorrectOrder(list);
            
            list.ForEach(x => x.Map());
            
            return new CursorPagedListDto<AudioDto, long>(list, query.Size, items => items[^1].Id);
        }
    }
}