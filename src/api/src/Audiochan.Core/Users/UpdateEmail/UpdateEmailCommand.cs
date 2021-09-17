using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using FluentValidation;
using MediatR;

namespace Audiochan.Core.Users.UpdateEmail
{
    public record UpdateEmailCommand : IRequest<Result>
    {
        public long UserId { get; init; }
        public string NewEmail { get; init; } = null!;

        public static UpdateEmailCommand FromRequest(long userId, UpdateEmailRequest request) => new()
        {
            UserId = userId,
            NewEmail = request.NewEmail
        };
    }

    public class UpdateEmailCommandValidator : AbstractValidator<UpdateEmailCommand>
    {
        public UpdateEmailCommandValidator()
        {
            RuleFor(req => req.NewEmail)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is invalid.");
        }
    }

    public class UpdateEmailCommandHandler : IRequestHandler<UpdateEmailCommand, Result>
    {
        private readonly long _currentUserId;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEmailCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateEmailCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);
            if (user == null) return Result.Unauthorized();
            if (user.Id != _currentUserId) return Result.Forbidden();

            user.Email = command.NewEmail;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}