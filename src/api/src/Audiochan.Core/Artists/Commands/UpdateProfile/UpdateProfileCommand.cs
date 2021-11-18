using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Attributes;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using FluentValidation;
using MediatR;

namespace Audiochan.Core.Artists.Commands
{
    [Authorize]
    public record UpdateProfileCommand : IRequest<Result<bool>>
    {
        public long UserId { get; init; }
        public string? DisplayName { get; init; }

        public static UpdateProfileCommand FromRequest(long userId, UpdateProfileRequest request) => new()
        {
            UserId = userId,
            DisplayName = request.DisplayName
        };
    }

    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator()
        {
            RuleFor(x => x.DisplayName)
                .NotEmpty();
        }
    }

    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<bool>>
    {
        private readonly long _currentUserId;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProfileCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            currentUserService.User.TryGetUserId(out _currentUserId);
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Artists.FindAsync(command.UserId, cancellationToken);
            if (user is null) return Result<bool>.NotFound();
            if (user.Id != _currentUserId)
                return Result<bool>.Forbidden();

            user.DisplayName = string.IsNullOrWhiteSpace(command.DisplayName)
                ? user.UserName
                : command.DisplayName;

            return Result<bool>.Success(true);
        }
    }
}