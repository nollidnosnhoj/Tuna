using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Exceptions;
using Audiochan.Application.Persistence;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Auth.Exceptions;
using Audiochan.Application.Features.Users.Exceptions;
using Audiochan.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Application.Features.Users.Commands.UpdateUsername
{
    public record UpdateUsernameCommand(long UserId, string NewUsername) : ICommandRequest;

    public class UpdateUsernameCommandHandler : IRequestHandler<UpdateUsernameCommand>
    {
        private readonly long _currentUserId;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;

        public UpdateUsernameCommandHandler(ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork, ApplicationDbContext dbContext)
        {
            currentUserService.User.TryGetUserId(out _currentUserId);
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(UpdateUsernameCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);
            if (user!.Id != _currentUserId) throw new ForbiddenException();
            
            // check if username already exists
            var usernameExists =
                await _dbContext.Users.AnyAsync(u => u.UserName == command.NewUsername, cancellationToken);
            if (usernameExists)
                throw new UsernameTakenException(command.NewUsername);

            // update username
            user.UserName = command.NewUsername;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}