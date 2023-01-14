﻿using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Common.Dtos;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Users.Commands.SetFollow
{
    public record SetFollowCommand(long ObserverId, long TargetId, bool IsFollowing) : ICommandRequest<Result>
    {
    }

    public class SetFollowCommandHandler : IRequestHandler<SetFollowCommand, Result>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUnitOfWork _unitOfWork;

        public SetFollowCommandHandler(IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
        {
            _dateTimeProvider = dateTimeProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(SetFollowCommand command, CancellationToken cancellationToken)
        {
            var target = await _unitOfWork.Users
                .LoadUserWithFollowers(command.TargetId, command.ObserverId, cancellationToken);

            if (target == null)
                return Result.NotFound<User>();

            if (target.Id == command.ObserverId)
                return Result.Forbidden();

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

            return Result.Success();
        }
    }
}