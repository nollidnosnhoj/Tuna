using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Exceptions;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Users.Commands
{
    public class UpdatePasswordCommand : AuthCommandRequest<IdentityResult>
    {
        public string CurrentPassword { get;  }
        public string NewPassword { get; }

        public UpdatePasswordCommand(string newPassword, string currentPassword, ClaimsPrincipal user) : base(user)
        {
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
        }
    }

    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, IdentityResult>
    {
        private readonly UserManager<User> _userManager;

        public UpdatePasswordCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
        {
            var userId = command.GetUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                throw new ResourceIdInvalidException<long>(typeof(User), userId);
            }

            if (user.Id != userId)
            {
                throw new ResourceOwnershipException<long>(typeof(User), user.Id, userId);
            }

            return await _userManager.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword);
        }
    }
}