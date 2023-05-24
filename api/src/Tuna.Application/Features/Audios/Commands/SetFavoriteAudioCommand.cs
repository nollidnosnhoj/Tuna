using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using MediatR;
using Tuna.Application.Features.Audios.Exceptions;
using Tuna.Application.Persistence;
using Tuna.Application.Services;
using Tuna.Domain.Entities;
using Tuna.Domain.Exceptions;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Audios.Commands;

public class SetFavoriteAudioCommand : ICommandRequest<Result<bool>>
{
    public SetFavoriteAudioCommand(long audioId, long userId, bool isFavoriting)
    {
        AudioId = audioId;
        UserId = userId;
        IsFavoriting = isFavoriting;
    }

    public long AudioId { get; }
    public long UserId { get; }
    public bool IsFavoriting { get; }
}

public class SetFavoriteAudioCommandHandler : IRequestHandler<SetFavoriteAudioCommand, Result<bool>>
{
    private readonly IClock _clock;
    private readonly IUnitOfWork _unitOfWork;

    public SetFavoriteAudioCommandHandler(IUnitOfWork unitOfWork, IClock clock)
    {
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<Result<bool>> Handle(SetFavoriteAudioCommand command,
        CancellationToken cancellationToken)
    {
        var audio = await _unitOfWork.Audios
            .LoadAudioWithFavorites(command.AudioId, command.UserId, cancellationToken);

        if (audio == null)
            return new Result<bool>(new AudioNotFoundException(command.AudioId));

        if (command.IsFavoriting)
            audio.Favorite(command.UserId, _clock.UtcNow);
        else
            audio.UnFavorite(command.UserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return command.IsFavoriting;
    }
}