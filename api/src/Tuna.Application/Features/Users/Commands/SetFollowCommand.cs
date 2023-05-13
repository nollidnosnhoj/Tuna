using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OneOf;
using OneOf.Types;
using Tuna.Application.Features.Users.Errors;
using Tuna.Application.Persistence;
using Tuna.Application.Services;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Users.Commands;

public record SetFollowCommand
    (long TargetId, long ObserverId, bool IsFollowing) : ICommandRequest<SetFollowCommandResult>;

[GenerateOneOf]
public partial class SetFollowCommandResult : OneOfBase<bool, NotFound, CannotFollowYourself>
{
}

public class SetFollowCommandHandler : IRequestHandler<SetFollowCommand, SetFollowCommandResult>
{
    private readonly IClock _clock;
    private readonly IUnitOfWork _unitOfWork;

    public SetFollowCommandHandler(IClock clock, IUnitOfWork unitOfWork)
    {
        _clock = clock;
        _unitOfWork = unitOfWork;
    }

    public async Task<SetFollowCommandResult> Handle(SetFollowCommand command, CancellationToken cancellationToken)
    {
        var target = await _unitOfWork.Users
            .LoadUserWithFollowers(command.TargetId, command.ObserverId, cancellationToken);

        if (target == null) return new NotFound();

        if (target.Id == command.ObserverId) return new CannotFollowYourself();

        if (command.IsFollowing)
            target.Follow(command.ObserverId, _clock.UtcNow);
        else
            target.UnFollow(command.ObserverId, _clock.UtcNow);

        _unitOfWork.Users.Update(target);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return command.IsFollowing;
    }
}