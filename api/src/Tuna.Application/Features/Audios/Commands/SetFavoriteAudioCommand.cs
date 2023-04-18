using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OneOf;
using OneOf.Types;
using Tuna.Application.Persistence;
using Tuna.Application.Services;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Audios.Commands;

public class SetFavoriteAudioCommand : ICommandRequest<SetFavoriteAudioResult>
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

[GenerateOneOf]
public partial class SetFavoriteAudioResult : OneOfBase<bool, NotFound>
{
}

public class SetFavoriteAudioCommandHandler : IRequestHandler<SetFavoriteAudioCommand, SetFavoriteAudioResult>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public SetFavoriteAudioCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<SetFavoriteAudioResult> Handle(SetFavoriteAudioCommand command,
        CancellationToken cancellationToken)
    {
        var audio = await _unitOfWork.Audios
            .LoadAudioWithFavorites(command.AudioId, command.UserId, cancellationToken);

        if (audio == null) return new NotFound();

        if (command.IsFavoriting)
            audio.Favorite(command.UserId, _dateTimeProvider.Now);
        else
            audio.UnFavorite(command.UserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return command.IsFavoriting;
    }
}