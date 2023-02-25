using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Exceptions;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Features.Auth;
using Audiochan.Core.Persistence;
using MediatR;

namespace Audiochan.Core.Features.Users.Commands
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
        private readonly IIdentityService _identityService;

        public UpdateUsernameCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        public async Task<bool> Handle(UpdateUsernameCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);
            
            if (user is null)
            {
                throw new UnauthorizedException();
            }

            var result = await _identityService.UpdateUserNameAsync(
                user.IdentityId,
                command.NewUserName,
                cancellationToken);
            
            result.EnsureSuccessfulResult();

            return result.IsSuccess;
        }
    }
}