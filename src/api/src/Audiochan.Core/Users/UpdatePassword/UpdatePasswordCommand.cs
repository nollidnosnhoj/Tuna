using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Users.UpdatePassword
{
    public record UpdatePasswordCommand : IRequest<Result>
    {
        public long UserId { get; init; }
        public string CurrentPassword { get; init; } = "";
        public string NewPassword { get; init; } = "";

        public static UpdatePasswordCommand FromRequest(long userId, UpdatePasswordRequest request) => new()
        {
            UserId = userId,
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword
        };
    }
    
    public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
    {
        public UpdatePasswordCommandValidator(IOptions<IdentitySettings> options)
        {
            RuleFor(req => req.NewPassword)
                .NotEmpty()
                .WithMessage("New Password is required.")
                .NotEqual(req => req.CurrentPassword)
                .WithMessage("New password cannot be the same as the previous.")
                .Password(options.Value.PasswordSettings, "New Password");
        }
    }

    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, Result>
    {
        private readonly long _currentUserId;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public UpdatePasswordCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, 
            IPasswordHasher passwordHasher)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);
            if (user == null) return Result<bool>.Unauthorized();
            if (user.Id != _currentUserId) return Result<bool>.Forbidden();
            if (!_passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
                return Result.BadRequest("Current password does not match.");   // Maybe give a generic error
            var newHash = _passwordHasher.Hash(command.NewPassword);
            user.PasswordHash = newHash;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}