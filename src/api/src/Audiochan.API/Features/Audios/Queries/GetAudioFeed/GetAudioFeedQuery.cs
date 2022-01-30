using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.CQRS;
using Audiochan.Core.Dtos;
using Audiochan.Core.Dtos.Wrappers;
using Audiochan.Core.Extensions;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Audios.Queries
{
    public record GetAudioFeedQuery(long UserId) : IHasOffsetPage, IQueryRequest<OffsetPagedListDto<AudioDto>>
    {
        public int Offset { get; init; }
        public int Size { get; init; }
    }

    public class GetAudioFeedQueryHandler : IRequestHandler<GetAudioFeedQuery, OffsetPagedListDto<AudioDto>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetAudioFeedQueryHandler(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<OffsetPagedListDto<AudioDto>> Handle(GetAudioFeedQuery query,
            CancellationToken cancellationToken)
        {
            var followingIds = await GetFollowingUserIdsAsync(query.UserId, cancellationToken);
            var list = await GetAudiosFromFollowings(followingIds, query.Offset, query.Size, cancellationToken);
            return new OffsetPagedListDto<AudioDto>(list, query.Offset, query.Size);
        }

        private async Task<long[]> GetFollowingUserIdsAsync(long targetId, CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Where(user => user.Id == targetId)
                .SelectMany(u => u.Followings.Select(f => f.TargetId))
                .ToArrayAsync(cancellationToken);
        }

        private async Task<List<AudioDto>> GetAudiosFromFollowings(long[] userIds, int offset, int limit, 
            CancellationToken cancellationToken)
        {
            return await _dbContext.Audios
                .AsNoTracking()
                .Where(a => userIds.Contains(a.UserId))
                .OrderByDescending(a => a.Created)
                .ProjectTo<AudioDto>(_mapper.ConfigurationProvider)
                .OffsetPaginateAsync(offset, limit, cancellationToken);
        }
    }
}