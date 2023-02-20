using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Common.Exceptions;
using Audiochan.Core.Features.Users.Exceptions;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Users.Commands.UpdatePassword
{
    public class UpdatePasswordCommand : AuthCommandRequest<bool>
    {
        public string CurrentPassword { get;  }
        public string NewPassword { get; }

        public UpdatePasswordCommand(string newPassword, string currentPassword, ClaimsPrincipal user) : base(user)
        {
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
        }
    }

    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public UpdatePasswordCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
        {
            var userId = command.GetUserId();
            var user = await _unitOfWork.Users.FindAsync(userId, cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedException();
            }

            if (user.Id != userId)
            {
                throw new UnauthorizedException();
            }

            if (!_passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
            {
                throw new PasswordDoesNotMatchException();
            }
            
            var newHash = _passwordHasher.Hash(command.NewPassword);
            user.PasswordHash = newHash;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}