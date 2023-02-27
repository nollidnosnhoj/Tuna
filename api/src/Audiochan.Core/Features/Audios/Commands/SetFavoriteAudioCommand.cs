using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Audiochan.Core.Features.Audios.Commands;

public class SetFavoriteAudioCommand : ICommandRequest<SetFavoriteAudioResult>
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

    public async Task<SetFavoriteAudioResult> Handle(SetFavoriteAudioCommand command, CancellationToken cancellationToken)
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