using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Exceptions;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Users.Commands
{
    public class UpdateEmailCommand : ICommandRequest<IdentityResult>
    {
        public long UserId { get; set; }
        public string NewEmail { get; }

        public UpdateEmailCommand(long userId, string newEmail)
        {
            NewEmail = newEmail;
        }
    }

    public class UpdateEmailCommandHandler : IRequestHandler<UpdateEmailCommand, IdentityResult>
    {
        private readonly UserManager<User> _userManager;

        public UpdateEmailCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> Handle(UpdateEmailCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(command.UserId.ToString());

            if (user is null)
            {
                throw new ResourceIdInvalidException<long>(typeof(User), command.UserId);
            }

            return await _userManager.SetEmailAsync(user, command.NewEmail);
        }
    }
}