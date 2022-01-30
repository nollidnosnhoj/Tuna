using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Services;
using Audiochan.Application.Persistence;
using Audiochan.Application.Commons.Extensions;
using MediatR;

namespace Audiochan.Application.Features.Users.Commands.UpdateProfile
{
    public record UpdateProfileCommand : ICommandRequest<Result<bool>>
    {
        public long UserId { get; init; }
        public string? DisplayName { get; init; }
        public string? About { get; init; }
        public string? Website { get; init; }

        public static UpdateProfileCommand FromRequest(long userId, UpdateProfileRequest request) => new()
        {
            UserId = userId,
            About = request.About,
            Website = request.Website,
            DisplayName = request.DisplayName
        };
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
            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);
            if (user!.Id != _currentUserId)
                return Result<bool>.Forbidden();
            
            // TODO: Update user stuff

            return Result<bool>.Success(true);
        }
    }
}