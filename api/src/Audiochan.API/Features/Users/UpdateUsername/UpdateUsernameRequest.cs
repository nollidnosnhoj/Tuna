using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.API.Features.Users.UpdateUsername
{
    public record UpdateUsernameRequest : IRequest<Result<bool>>
    {
        [JsonIgnore] public string UserId { get; init; } = string.Empty;
        public string NewUsername { get; init; } = null!;
    }

    public class UpdateUsernameRequestHandler : IRequestHandler<UpdateUsernameRequest, Result<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;

        public UpdateUsernameRequestHandler(ICurrentUserService currentUserService, IIdentityService identityService, 
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateUsernameRequest request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(new object[]{request.UserId}, cancellationToken);
            if (user == null) return Result<bool>.Fail(ResultError.Unauthorized);
            if (user.Id != _currentUserService.GetUserId())
                return Result<bool>.Fail(ResultError.Forbidden);

            // update username
            return await _identityService.UpdateUsername(user, request.NewUsername);
        }
    }
}