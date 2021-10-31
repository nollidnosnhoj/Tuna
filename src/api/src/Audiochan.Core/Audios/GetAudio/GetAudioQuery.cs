using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Audiochan.Core.Audios
{
    public record GetAudioQuery(long Id) : IRequest<AudioDto?>
    {
    }

    public class GetAudioQueryHandler : IRequestHandler<GetAudioQuery, AudioDto?>
    {
        private readonly IDistributedCache _cache;
        private readonly IUnitOfWork _unitOfWork;

        public GetAudioQueryHandler(IUnitOfWork unitOfWork, IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<AudioDto?> Handle(GetAudioQuery query, CancellationToken cancellationToken)
        {
            var key = CacheKeys.Audio.GetAudio(query.Id);
            return await _cache.GetOrCreateAsync(
                key: key,
                factory: async () => await GetAudioFromDatabase(query.Id, cancellationToken), 
                cachingOptions: new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                }, 
                cancellationToken: cancellationToken);
        }

        private async Task<AudioDto?> GetAudioFromDatabase(long audioId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Audios
                .GetFirstAsync<AudioDto>(new GetAudioSpecification(audioId), cancellationToken);
        }
    }
}