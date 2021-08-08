using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Users.UpdatePassword
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
        private readonly ApplicationDbContext _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public UpdatePasswordCommandHandler(ICurrentUserService currentUserService, ApplicationDbContext unitOfWork, 
            IPasswordHasher passwordHasher)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(new object[]{command.UserId}, cancellationToken);
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