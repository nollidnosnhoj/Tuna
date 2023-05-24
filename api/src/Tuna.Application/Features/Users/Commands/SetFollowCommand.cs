using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using MediatR;
using Tuna.Application.Features.Users.Exceptions;
using Tuna.Application.Persistence;
using Tuna.Application.Services;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Users.Commands;

public record SetFollowCommand
    (long TargetId, long ObserverId, bool IsFollowing) : ICommandRequest<Result<bool>>;

public class SetFollowCommandHandler : IRequestHandler<SetFollowCommand, Result<bool>>
{
    private readonly IClock _clock;
    private readonly IUnitOfWork _unitOfWork;

    public SetFollowCommandHandler(IClock clock, IUnitOfWork unitOfWork)
    {
        _clock = clock;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(SetFollowCommand command, CancellationToken cancellationToken)
    {
        var target = await _unitOfWork.Users
            .LoadUserWithFollowers(command.TargetId, command.ObserverId, cancellationToken);

        if (target == null)
            return new Result<bool>(new UserNotFoundException(command.TargetId));

        if (target.Id == command.ObserverId)
            return new Result<bool>(new FollowingException(command.TargetId, "You can't follow yourself."));

        if (command.IsFollowing)
            target.Follow(command.ObserverId, _clock.UtcNow);
        else
            target.UnFollow(command.ObserverId, _clock.UtcNow);

        _unitOfWork.Users.Update(target);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return command.IsFollowing;
    }
}