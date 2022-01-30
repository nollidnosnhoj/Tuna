using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using DistributedCacheExtensions = Audiochan.Application.Commons.Extensions.DistributedCacheExtensions;

namespace Audiochan.Application.Features.Audios.Queries.GetAudio
{
    public record GetAudioQuery(long Id) : IQueryRequest<AudioDto?>
    {
    }

    public class GetAudioQueryHandler : IRequestHandler<GetAudioQuery, AudioDto?>
    {
        private readonly IDistributedCache _cache;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetAudioQueryHandler(IDistributedCache cache, ApplicationDbContext dbContext, IMapper mapper)
        {
            _cache = cache;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<AudioDto?> Handle(GetAudioQuery query, CancellationToken cancellationToken)
        {
            var key = CacheKeys.Audio.GetAudio(query.Id);
            return await Commons.Extensions.DistributedCacheExtensions.GetOrCreateAsync(_cache, key: key,
                factory: async () => await GetAudioFromDatabase(query.Id, cancellationToken), 
                cachingOptions: new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                }, 
                cancellationToken: cancellationToken);
        }

        private async Task<AudioDto?> GetAudioFromDatabase(long audioId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Audios
                .AsNoTracking()
                .Where(a => a.Id == audioId)
                .ProjectTo<AudioDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}