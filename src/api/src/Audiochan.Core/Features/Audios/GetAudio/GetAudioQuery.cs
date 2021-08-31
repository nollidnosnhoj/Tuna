using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.GetAudio
{
    public record GetAudioQuery(long Id) : IRequest<AudioDto?>
    {
    }

    public class GetAudioQueryHandler : IRequestHandler<GetAudioQuery, AudioDto?>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICacheService _cacheService;
        private readonly long _currentUserId;

        public GetAudioQueryHandler(ICacheService cacheService, ICurrentUserService currentUserService, ApplicationDbContext dbContext)
        {
            _cacheService = cacheService;
            _dbContext = dbContext;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<AudioDto?> Handle(GetAudioQuery query, CancellationToken cancellationToken)
        {
            var cacheOptions = new GetAudioCacheOptions(query.Id);
            
            var (cacheExists, audio) = await _cacheService
                .GetAsync<AudioDto>(cacheOptions, cancellationToken);

            if (cacheExists) return audio;
            
            audio = await _dbContext.Audios
                .AsNoTracking()
                .Where(x => x.Id == query.Id)
                .Select(AudioMaps.AudioToView())
                .SingleOrDefaultAsync(cancellationToken);
            
            await _cacheService.SetAsync(audio, cacheOptions, cancellationToken);

            return audio;
        }
    }
}