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
    public record GetAudioQuery(long Id, string? PrivateKey = null) : IRequest<AudioDetailViewModel?>
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
                audio = await _unitOfWork.Audios.GetAudio(query.Id, cancellationToken);
            
            if (audio == null) return null;

            if (ShouldNotAccessPrivateAudio(audio, query.PrivateKey)) return null;

            if (!cacheExists)
                await _cacheService.SetAsync(audio, cacheOptions, cancellationToken);

            return audio;
        }

        private bool ShouldNotAccessPrivateAudio(AudioDetailViewModel audio, string? privateKey)
        {
            var currentUserId = _currentUserService.GetUserId();

            return audio.Visibility == Visibility.Private
                   && audio.PrivateKey != privateKey
                   && currentUserId != audio.User.Id;
        }
    }
}