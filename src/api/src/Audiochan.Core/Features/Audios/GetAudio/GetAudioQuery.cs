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
    public record GetAudioQuery : IRequest<AudioViewModel?>
    {
        public long Id { get; init; }
        public string? Secret { get; init; }

        public GetAudioQuery(long id, string? secret = null)
        {
            Id = id;
            Secret = secret;
        }
    }

    public class GetAudioQueryHandler : IRequestHandler<GetAudioQuery, AudioViewModel?>
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

        public async Task<AudioViewModel?> Handle(GetAudioQuery query, CancellationToken cancellationToken)
        {
            var audio = await FetchAudioFromCacheOrDatabaseAsync(query.Id, cancellationToken);
            if (audio == null || !CanAccessPrivateAudio(audio, query.Secret)) return null;
            return audio;
        }

        private async Task<AudioViewModel?> FetchAudioFromCacheOrDatabaseAsync(long audioId, 
            CancellationToken cancellationToken = default)
        {
            var cacheOptions = new GetAudioCacheOptions(audioId);
            
            var (cacheExists, audio) = await _cacheService
                .GetAsync<AudioViewModel>(cacheOptions, cancellationToken);

            if (!cacheExists)
            {
                audio = await _dbContext.Audios
                    .AsNoTracking()
                    .Where(x => x.Id == audioId)
                    .Select(AudioMaps.AudioToView())
                    .SingleOrDefaultAsync(cancellationToken);
                await _cacheService.SetAsync(audio, cacheOptions, cancellationToken);
            }

            return audio;
        }

        private bool CanAccessPrivateAudio(AudioViewModel audio, string? secret)
        {
            return _currentUserId == audio.User.Id || audio.Visibility != Visibility.Private || audio.Secret == secret;
        }
    }
}