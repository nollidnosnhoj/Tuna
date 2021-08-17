using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.FavoriteAudios.SetFavoriteAudio
{
    public record SetFavoriteAudioCommand(long AudioId, long UserId, bool IsFavoriting) : IRequest<Result<bool>>
    {
    }
    
    public class SetFavoriteAudioCommandHandler : IRequestHandler<SetFavoriteAudioCommand, Result<bool>>
    {
        private readonly ApplicationDbContext _unitOfWork;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SetFavoriteAudioCommandHandler(ApplicationDbContext unitOfWork, IDateTimeProvider dateTimeProvider)
        {
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result<bool>> Handle(SetFavoriteAudioCommand command, CancellationToken cancellationToken)
        {
            var queryable = _unitOfWork.Audios
                .IgnoreQueryFilters()
                .Where(a => a.Id == command.AudioId);

            queryable = UserHelpers.IsValidId(command.UserId)
                ? queryable.Include(a => 
                    a.Favorited.Where(fa => fa.Id == command.UserId)) 
                : queryable.Include(a => a.Favorited);

            var audio = await queryable.SingleOrDefaultAsync(cancellationToken);

            if (audio == null)
                return Result<bool>.NotFound<Audio>();

            var isFavoriting = command.IsFavoriting
                ? await Favorite(audio, command.UserId, cancellationToken)
                : await Unfavorite(audio, command.UserId, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(isFavoriting);
        }
        
        private async Task<bool> Favorite(Audio target, long userId, CancellationToken cancellationToken = default)
        {
            var favoriter = target.Favorited.FirstOrDefault(f => f.Id == userId);

            if (favoriter is null)
            {
                await _unitOfWork.FavoriteAudios.AddAsync(new FavoriteAudio
                {
                    UserId = userId,
                    AudioId = target.Id
                }, cancellationToken);
            }
            
            return true;
        }

        private Task<bool> Unfavorite(Audio target, long userId, CancellationToken cancellationToken = default)
        {
            var favoriter = target.Favorited.FirstOrDefault(f => f.Id == userId);

            if (favoriter is not null)
            {
                target.Favorited.Remove(favoriter);
            }

            return Task.FromResult(false);
        }
    }
}