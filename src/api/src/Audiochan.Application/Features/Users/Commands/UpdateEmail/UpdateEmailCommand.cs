using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Exceptions;
using Audiochan.Application.Commons.Services;
using Audiochan.Application.Persistence;
using Audiochan.Application.Commons.Extensions;
using MediatR;

namespace Audiochan.Application.Features.Users.Commands.UpdateEmail
{
    public record UpdateEmailCommand : ICommandRequest
    {
        public long UserId { get; init; }
        public string NewEmail { get; init; } = null!;

        public static UpdateEmailCommand FromRequest(long userId, UpdateEmailRequest request) => new()
        {
            UserId = userId,
            NewEmail = request.NewEmail
        };
    }

    public class UpdateEmailCommandHandler : IRequestHandler<UpdateEmailCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEmailCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateEmailCommand command, CancellationToken cancellationToken)
        {
            _currentUserService.User.TryGetUserId(out var currentUserId);

            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);
            if (user!.Id != currentUserId) throw new ForbiddenException();

            user.Email = command.NewEmail;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}