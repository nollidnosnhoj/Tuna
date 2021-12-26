using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Exceptions;
using Audiochan.Application.Commons.Services;
using Audiochan.Application.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Application.Features.Users.Commands.SetFollow
{
    public record SetFollowCommand(long ObserverId, long TargetId, bool IsFollowing) : ICommandRequest
    {
    }

    public class SetFollowCommandHandler : IRequestHandler<SetFollowCommand>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUnitOfWork _unitOfWork;

        public SetFollowCommandHandler(IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
        {
            _dateTimeProvider = dateTimeProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(SetFollowCommand command, CancellationToken cancellationToken)
        {
            var target = await _unitOfWork.Users
                .LoadUserWithFollowers(command.TargetId, command.ObserverId, cancellationToken);

            if (target == null)
                throw new NotFoundException<User, long>(command.TargetId);

            if (target.Id == command.ObserverId)
                throw new ForbiddenException();

            if (command.IsFollowing)
            {
                target.Follow(command.ObserverId, _dateTimeProvider.Now);
            }
            else
            {
                target.UnFollow(command.ObserverId, _dateTimeProvider.Now);
            }

            _unitOfWork.Users.Update(target);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}