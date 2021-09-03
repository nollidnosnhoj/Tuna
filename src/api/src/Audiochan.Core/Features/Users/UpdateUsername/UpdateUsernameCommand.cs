using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Users.UpdateUsername
{
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
            RuleFor(req => req.NewUsername).Username(options.Value.UsernameSettings);
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
            if (user == null) return Result.Unauthorized();
            if (user.Id != _currentUserId) return Result.Forbidden();
            
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