using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Common.Exceptions;
using Audiochan.Core.Features.Users.Exceptions;
using Audiochan.Core.Persistence;
using MediatR;

namespace Audiochan.Core.Features.Users.Commands.UpdateEmail
{
    public class UpdateEmailCommand : ICommandRequest<bool>
    {
        public long UserId { get; set; }
        public string NewEmail { get; }

        public UpdateEmailCommand(long userId, string newEmail)
        {
            NewEmail = newEmail;
        }
    }

    public class UpdateEmailCommandHandler : IRequestHandler<UpdateEmailCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEmailCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateEmailCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedException();
            }

            if (await _unitOfWork.Users.CheckIfEmailExists(command.NewEmail, cancellationToken))
            {
                throw new DuplicateEmailException(command.NewEmail);
            }

            user.Email = command.NewEmail;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}