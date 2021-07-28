using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.GetAudio
{
    public record GetAudioQuery(Guid Id) : IRequest<AudioViewModel?>
    {
    }

    public class GetAudioQueryHandler : IRequestHandler<GetAudioQuery, AudioViewModel?>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICacheService _cacheService;
        private readonly ICurrentUserService _currentUserService;

        public GetAudioQueryHandler(ICacheService cacheService, ICurrentUserService currentUserService, ApplicationDbContext dbContext)
        {
            _cacheService = cacheService;
            _currentUserService = currentUserService;
            _dbContext = dbContext;
        }

        public async Task<AudioViewModel?> Handle(GetAudioQuery query, CancellationToken cancellationToken)
        {
            var audio = await FetchAudioFromCacheOrDatabaseAsync(query.Id, cancellationToken);
            if (audio == null || !CanAccessPrivateAudio(audio)) return null;
            return audio;
        }

        private async Task<AudioViewModel?> FetchAudioFromCacheOrDatabaseAsync(Guid audioId, 
            CancellationToken cancellationToken = default)
        {
            var cacheOptions = new GetAudioCacheOptions(audioId);
            
            var (cacheExists, audio) = await _cacheService
                .GetAsync<AudioViewModel>(cacheOptions, cancellationToken);

            if (!cacheExists)
            {
                audio = await _dbContext.Audios
                    .AsNoTracking()
                    .Include(x => x.Tags)
                    .Include(x => x.User)
                    .Where(x => x.Id == audioId)
                    .Select(AudioMaps.AudioToView)
                    .SingleOrDefaultAsync(cancellationToken);
                await _cacheService.SetAsync(audio, cacheOptions, cancellationToken);
            }

            return audio;
        }

        private bool CanAccessPrivateAudio(AudioViewModel audio)
        {
            var currentUserId = _currentUserService.GetUserId();

            return currentUserId == audio.User.Id || audio.Visibility != Visibility.Private;
        }
    }
}