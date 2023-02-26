using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Exceptions;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Features.Auth;
using Audiochan.Core.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Users.Commands
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
        private readonly IIdentityService _identityService;

        public UpdatePasswordCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        public async Task<bool> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
        {
            var userId = command.GetUserId();
            var user = await _unitOfWork.Users.FindAsync(userId, cancellationToken);

            if (user is null)
            {
                throw new ResourceIdInvalidException<long>(typeof(User), userId);
            }

            if (user.Id != userId)
            {
                throw new ResourceOwnershipException<long>(typeof(User), user.Id, userId);
            }

            var result = await _identityService.UpdatePasswordAsync(
                user.IdentityId, 
                command.CurrentPassword,
                command.NewPassword, 
                cancellationToken);
            
            result.EnsureSuccessfulResult();
            
            // TODO: Remove session

            return result.IsSuccess;
        }
    }
}