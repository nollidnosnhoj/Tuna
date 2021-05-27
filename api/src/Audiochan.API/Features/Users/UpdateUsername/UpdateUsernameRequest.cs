using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;

        public UpdateUsernameRequestHandler(ICurrentUserService currentUserService, IUserRepository userRepository, 
            IIdentityService identityService)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _identityService = identityService;
        }

        public async Task<Result<bool>> Handle(UpdateUsernameRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null) return Result<bool>.Fail(ResultError.Unauthorized);
            if (user.Id != _currentUserService.GetUserId())
                return Result<bool>.Fail(ResultError.Forbidden);

            // update username
            return await _identityService.UpdateUsername(user, request.NewUsername);
        }
    }
}