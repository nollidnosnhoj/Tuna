using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Features.Audios.Exceptions;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Users.Commands.SetFavoriteAudio
{
    public class SetFavoriteAudioCommand : ICommandRequest<bool>
    {
        public long AudioId { get; }
        public long UserId { get; }
        public bool IsFavoriting { get; }
        public SetFavoriteAudioCommand(long audioId, long userId, bool isFavoriting)
        {
            AudioId = audioId;
            UserId = userId;
            IsFavoriting = isFavoriting;
        }
    }

    public class SetFavoriteAudioCommandHandler : IRequestHandler<SetFavoriteAudioCommand, bool>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUnitOfWork _unitOfWork;

        public SetFavoriteAudioCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
        {
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<bool> Handle(SetFavoriteAudioCommand command, CancellationToken cancellationToken)
        {
            var audio = await _unitOfWork.Audios
                .LoadAudioWithFavorites(command.AudioId, command.UserId, cancellationToken);

            if (audio == null)
            {
                throw new AudioNotFoundException(command.AudioId);
            }

            if (command.IsFavoriting)
                audio.Favorite(command.UserId, _dateTimeProvider.Now);
            else
                audio.UnFavorite(command.UserId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return command.IsFavoriting;
        }
    }
}