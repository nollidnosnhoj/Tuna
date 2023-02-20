using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Common.Dtos;
using Audiochan.Core.Features.Users.Exceptions;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Users.Commands.SetFollow
{
    public record SetFollowCommand(long TargetId, long ObserverId, bool IsFollowing) : ICommandRequest<bool>
    {
    }

    public class SetFollowCommandHandler : IRequestHandler<SetFollowCommand, bool>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUnitOfWork _unitOfWork;

        public SetFollowCommandHandler(IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
        {
            _dateTimeProvider = dateTimeProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(SetFollowCommand command, CancellationToken cancellationToken)
        {
            var target = await _unitOfWork.Users
                .LoadUserWithFollowers(command.TargetId, command.ObserverId, cancellationToken);

            if (target == null)
            {
                throw new UserNotFoundException(command.TargetId);
            }

            if (target.Id == command.ObserverId)
            {
                throw new CannotFollowYourselfException();
            }

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

            return command.IsFollowing;
        }
    }
}