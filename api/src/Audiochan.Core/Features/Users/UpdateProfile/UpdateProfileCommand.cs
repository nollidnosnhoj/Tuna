using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Users.UpdateProfile
{
    public record UpdateProfileCommand : IRequest<Result<bool>>
    {
        public string UserId { get; init; } = string.Empty;
        public string? DisplayName { get; init; }
        public string? About { get; init; }
        public string? Website { get; init; }

        public static UpdateProfileCommand FromRequest(string userId, UpdateProfileRequest request) => new()
        {
            UserId = userId,
            About = request.About,
            Website = request.Website,
            DisplayName = request.DisplayName
        };
    }

    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProfileCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.LoadAsync(new object[]{command.UserId}, cancellationToken);
            if (user == null) return Result<bool>.NotFound<User>();
            if (user.Id != _currentUserService.GetUserId())
                return Result<bool>.Forbidden();

            user.UpdateDisplayName(command.DisplayName);
            user.UpdateAbout(command.About);
            user.UpdateWebsite(command.Website);

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}