using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Audios.Mappings;
using Audiochan.Core.CQRS;
using Audiochan.Core.Dtos;
using Audiochan.Core.Dtos.Wrappers;
using Audiochan.Core.Extensions;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Audios.Queries
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

            var list = await queryable
                .OrderByDescending(a => a.Id)
                .Project(currentUserId)
                .CursorPaginateAsync(query.Cursor, query.Size, cancellationToken);
            
            list.ForEach(x => x.Map());
            
            return new CursorPagedListDto<AudioDto, long>(list, query.Size);
        }
    }
}