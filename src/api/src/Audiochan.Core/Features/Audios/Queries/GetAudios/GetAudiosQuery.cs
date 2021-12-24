using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Commons.CQRS;
using Audiochan.Core.Commons.Dtos.Wrappers;
using Audiochan.Core.Commons.Extensions;
using Audiochan.Core.Commons.Interfaces;

using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        private readonly IMapper _mapper;

        public GetAudiosQueryHandler(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<CursorPagedListDto<AudioDto, long>> Handle(GetAudiosQuery query,
            CancellationToken cancellationToken)
        {
            var queryable = _dbContext.Audios.AsNoTracking();

            if (query.Tags.Count > 0)
                queryable = queryable.Where(a => a.Tags.Any(t => query.Tags.Contains(t)));

            var list = await queryable
                .OrderByDescending(a => a.Id)
                .ProjectTo<AudioDto>(_mapper.ConfigurationProvider)
                .CursorPaginateAsync(query.Cursor, query.Size, cancellationToken);
            
            return new CursorPagedListDto<AudioDto, long>(list, query.Size);
        }
    }
}