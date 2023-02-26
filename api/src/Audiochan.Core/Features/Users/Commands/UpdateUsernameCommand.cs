using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Exceptions;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Auth;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Users.Commands
{
    public class UpdateUsernameCommand : ICommandRequest<IdentityResult>
    {
        public long UserId { get; }
        public string NewUserName { get; }

        public UpdateUsernameCommand(long userId, string newUserName)
        {
            UserId = userId;
            NewUserName = newUserName;
        }
    }

    public class UpdateUsernameCommandHandler : IRequestHandler<UpdateUsernameCommand, IdentityResult>
    {
        private readonly UserManager<User> _userManager;

        public UpdateUsernameCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> Handle(UpdateUsernameCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(command.UserId.ToString());
            
            if (user is null)
            {
                throw new ResourceIdInvalidException<long>(typeof(User), command.UserId);
            }

            return await _userManager.SetUserNameAsync(user, command.NewUserName);
        }
    }
}