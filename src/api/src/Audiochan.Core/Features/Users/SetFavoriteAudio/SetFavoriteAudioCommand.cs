using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Users.SetFavoriteAudio
{
    public record SetFavoriteAudioCommand(long AudioId, long UserId, bool IsFavoriting) : IRequest<Result<bool>>
    {
    }

    public sealed class LoadAudioForFavoritingSpecification : Specification<Audio>
    {
        public LoadAudioForFavoritingSpecification(long audioId, long observerId)
        {
            if (observerId > 0)
            {
                Query.Include(a =>
                    a.FavoriteAudios.Where(fa => fa.UserId == observerId));
            }
            else
            {
                Query.Include(a => a.FavoriteAudios);
            }

            Query.Where(a => a.Id == audioId);
        }
    }

    public class SetFavoriteAudioCommandHandler : IRequestHandler<SetFavoriteAudioCommand, Result<bool>>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUnitOfWork _unitOfWork;

        public SetFavoriteAudioCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
        {
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result<bool>> Handle(SetFavoriteAudioCommand command, CancellationToken cancellationToken)
        {
            var audio = await _unitOfWork.Audios
                .GetFirstAsync(new LoadAudioForFavoritingSpecification(command.AudioId, command.UserId), cancellationToken);

            if (audio == null)
                return Result<bool>.NotFound<Audio>();

            var isFavoriting = command.IsFavoriting
                ? Favorite(audio, command.UserId, cancellationToken)
                : Unfavorite(audio, command.UserId, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(isFavoriting);
        }
        
        private bool Favorite(Audio target, long userId, CancellationToken cancellationToken = default)
        {
            var favoriteAudio = target.FavoriteAudios.FirstOrDefault(f => f.UserId == userId);

            if (favoriteAudio is null)
            {
                target.FavoriteAudios.Add(new FavoriteAudio
                {
                    UserId = userId,
                    AudioId = target.Id,
                    Favorited = _dateTimeProvider.Now
                });
                // await _unitOfWork.FavoriteAudios.AddAsync(new FavoriteAudio
                // {
                //     UserId = userId,
                //     AudioId = target.Id
                // }, cancellationToken);
            }
            
            return true;
        }

        private bool Unfavorite(Audio target, long userId, CancellationToken cancellationToken = default)
        {
            var favoriteAudio = target.FavoriteAudios.FirstOrDefault(f => f.UserId == userId);

            if (favoriteAudio is not null)
            {
                target.FavoriteAudios.Remove(favoriteAudio);
            }

            return false;
        }
    }
}