using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Common.Exceptions;
using Audiochan.Core.Features.Users.Exceptions;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.Commands.UpdateUsername
{
    public class UpdateUsernameCommand : ICommandRequest<bool>
    {
        public long UserId { get; }
        public string NewUserName { get; }

        public UpdateUsernameCommand(long userId, string newUserName)
        {
            UserId = userId;
            NewUserName = newUserName;
        }
    }

    public class UpdateUsernameCommandHandler : IRequestHandler<UpdateUsernameCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUsernameCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateUsernameCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);
            
            if (user is null)
            {
                throw new UnauthorizedException();
            }
            
            // check if username already exists
            var usernameExists = await _unitOfWork.Users.CheckIfUsernameExists(command.NewUserName, cancellationToken);
            
            if (usernameExists)
            {
                throw new DuplicateUserNameException(command.NewUserName);
            }

            // update username
            user.UserName = command.NewUserName;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}