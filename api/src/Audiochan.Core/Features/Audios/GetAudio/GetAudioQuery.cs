using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Audios.GetAudio
{
    public record GetAudioQuery(Guid Id) : IRequest<AudioDetailViewModel?>
    {
    }

    public class GetAudioQueryHandler : IRequestHandler<GetAudioQuery, AudioDetailViewModel?>
    {
        private readonly ICacheService _cacheService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetAudioQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _currentUserService = currentUserService;
        }

        public async Task<AudioDetailViewModel?> Handle(GetAudioQuery query, CancellationToken cancellationToken)
        {
            var cacheOptions = new GetAudioCacheOptions(query.Id);
            
            var (cacheExists, audio) = await _cacheService.GetAsync<AudioDetailViewModel>(cacheOptions, cancellationToken);

            if (!cacheExists)
            {
                audio = await _unitOfWork.Audios.GetAudio(query.Id, cancellationToken);
                await _cacheService.SetAsync(audio, cacheOptions, cancellationToken);
            }
            
            if (audio == null || !CanAccessPrivateAudio(audio)) return null;

            return audio;
        }

        private bool CanAccessPrivateAudio(AudioDetailViewModel audio)
        {
            var currentUserId = _currentUserService.GetUserId();

            return currentUserId == audio.User.Id || audio.Visibility != Visibility.Private;
        }
    }
}