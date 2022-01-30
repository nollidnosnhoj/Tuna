using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Services;
using Audiochan.Application.Persistence;
using Audiochan.Application.Commons.Extensions;
using MediatR;

namespace Audiochan.Application.Features.Users.Commands.UpdatePassword
{
    public record UpdatePasswordCommand : ICommandRequest<Result>
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

    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, Result>
    {
        private readonly long _currentUserId;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public UpdatePasswordCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, 
            IPasswordHasher passwordHasher)
        {
            currentUserService.User.TryGetUserId(out _currentUserId);
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);
            if (user!.Id != _currentUserId) return Result<bool>.Forbidden();
            if (!_passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
                return Result.BadRequest("Current password does not match.");   // Maybe give a generic error
            var newHash = _passwordHasher.Hash(command.NewPassword);
            user.PasswordHash = newHash;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}