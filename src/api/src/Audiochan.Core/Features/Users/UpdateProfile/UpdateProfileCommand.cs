using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Users.UpdateProfile
{
    public record UpdateProfileCommand : IRequest<Result<bool>>
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
        private readonly ApplicationDbContext _unitOfWork;

        public UpdateProfileCommandHandler(ICurrentUserService currentUserService, ApplicationDbContext unitOfWork)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(new object[]{command.UserId}, cancellationToken);
            if (user == null) return Result<bool>.NotFound<User>();
            if (user.Id != _currentUserId)
                return Result<bool>.Forbidden();
            
            // TODO: Update user stuff

            return Result<bool>.Success(true);
        }
    }
}