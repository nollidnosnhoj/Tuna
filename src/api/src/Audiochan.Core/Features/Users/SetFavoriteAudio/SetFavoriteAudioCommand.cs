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
    public record SetFavoriteAudioCommand(long AudioId, long UserId, bool IsFavoriting) : IRequest<Result>
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

    public class SetFavoriteAudioCommandHandler : IRequestHandler<SetFavoriteAudioCommand, Result>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUnitOfWork _unitOfWork;

        public SetFavoriteAudioCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
        {
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result> Handle(SetFavoriteAudioCommand command, CancellationToken cancellationToken)
        {
            var audio = await _unitOfWork.Audios
                .GetFirstAsync(new LoadAudioForFavoritingSpecification(command.AudioId, command.UserId), cancellationToken);

            if (audio == null)
                return Result.NotFound<Audio>();

            if (command.IsFavoriting)
                audio.Favorite(command.UserId, _dateTimeProvider.Now);
            else
                audio.UnFavorite(command.UserId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}