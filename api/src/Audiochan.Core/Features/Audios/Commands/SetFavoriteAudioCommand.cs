using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Exceptions;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Entities;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Audios.Commands
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
                throw new ResourceIdInvalidException<long>(typeof(Audio), command.AudioId);
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