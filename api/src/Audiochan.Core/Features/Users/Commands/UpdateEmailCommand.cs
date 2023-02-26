using System;
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
        private readonly IIdentityService _identityService;

        public UpdateEmailCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        public async Task<bool> Handle(UpdateEmailCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

            if (user is null)
            {
                throw new ResourceIdInvalidException<long>(typeof(User), command.UserId);
            }

            var result = await _identityService.UpdateEmailAsync(user.IdentityId, command.NewEmail, cancellationToken);
            
            result.EnsureSuccessfulResult();

            return result.IsSuccess;
        }
    }
}