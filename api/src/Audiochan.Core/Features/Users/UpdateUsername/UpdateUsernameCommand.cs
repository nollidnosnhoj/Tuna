using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Users.UpdateUsername
{
    public record UpdateUsernameCommand : IRequest<Result<bool>>
    {
        public string UserId { get; init; } = null!;
        public string NewUsername { get; init; } = null!;

        public static UpdateUsernameCommand FromRequest(string userId, UpdateUsernameRequest request) => new()
        {
            UserId = userId,
            NewUsername = request.NewUsername
        };
    }

    public class UpdateUsernameCommandHandler : IRequestHandler<UpdateUsernameCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;

        public UpdateUsernameCommandHandler(ICurrentUserService currentUserService, IIdentityService identityService, 
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateUsernameCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(new object[]{command.UserId}, cancellationToken);
            if (user == null) return Result<bool>.Fail(ResultError.Unauthorized);
            if (user.Id != _currentUserService.GetUserId())
                return Result<bool>.Fail(ResultError.Forbidden);

            // update username
            return await _identityService.UpdateUsername(user, command.NewUsername);
        }
    }
}