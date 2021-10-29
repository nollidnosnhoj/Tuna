using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Attributes;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Users.UpdateUsername
{
    [Authorize]
    public record UpdateUsernameCommand : IRequest<Result>
    {
        public long UserId { get; init; }
        public string NewUsername { get; init; } = null!;

        public static UpdateUsernameCommand FromRequest(long userId, UpdateUsernameRequest request) => new()
        {
            UserId = userId,
            NewUsername = request.NewUsername
        };
    }
    
    public class UpdateUsernameCommandValidator : AbstractValidator<UpdateUsernameCommand>
    {
        public UpdateUsernameCommandValidator(IOptions<IdentitySettings> options)
        {
            RuleFor(req => req.NewUsername).UsernameValidation(options.Value.UsernameSettings);
        }
    }

    public class UpdateUsernameCommandHandler : IRequestHandler<UpdateUsernameCommand, Result>
    {
        private readonly long _currentUserId;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUsernameCommandHandler(ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateUsernameCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);
            if (user!.Id != _currentUserId) return Result.Forbidden();
            
            // check if username already exists
            var usernameExists =
                await _unitOfWork.Users.ExistsAsync(u => u.UserName == command.NewUsername, cancellationToken);
            if (usernameExists)
                return Result.BadRequest("Username already exists");

            // update username
            user.UserName = command.NewUsername;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}