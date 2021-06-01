using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Models;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.API.Features.FavoriteAudios.SetFavorite
{
    public record SetFavoriteAudioRequest(Guid AudioId, string UserId, bool IsFavoriting) : IRequest<Result<bool>>
    {
    }
    
    public class SetFavoriteAudioRequestHandler : IRequestHandler<SetFavoriteAudioRequest, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SetFavoriteAudioRequestHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
        {
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result<bool>> Handle(SetFavoriteAudioRequest request, CancellationToken cancellationToken)
        {
            var audio = await _unitOfWork.Audios.GetAsync(
                new GetTargetAudioForFavoritingSpecification(request.AudioId), true, cancellationToken);
            
            if (audio == null)
                return Result<bool>.Fail(ResultError.NotFound);

            var isFavoriting = request.IsFavoriting
                ? await Favorite(audio, request.UserId, cancellationToken)
                : await Unfavorite(audio, request.UserId, cancellationToken);

            _unitOfWork.Audios.Update(audio);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(isFavoriting);
        }
        
        private Task<bool> Favorite(Audio target, string userId, CancellationToken cancellationToken = default)
        {
            var favoriter = target.Favorited.FirstOrDefault(f => f.UserId == userId);

            if (favoriter is null)
            {
                favoriter = new FavoriteAudio
                {
                    AudioId = target.Id,
                    UserId = userId,
                    FavoriteDate = _dateTimeProvider.Now
                };
                
                target.Favorited.Add(favoriter);
            }
            else if (favoriter.UnfavoriteDate is not null)
            {
                favoriter.FavoriteDate = _dateTimeProvider.Now;
                favoriter.UnfavoriteDate = null;
            }
            
            return Task.FromResult(true);
        }

        private Task<bool> Unfavorite(Audio target, string userId, CancellationToken cancellationToken = default)
        {
            var favoriter = target.Favorited.FirstOrDefault(f => f.UserId == userId);

            if (favoriter is not null)
            {
                favoriter.UnfavoriteDate = _dateTimeProvider.Now;
            }

            return Task.FromResult(false);
        }
    }
}