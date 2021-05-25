using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Users.UpdateUsername
{
    public record UpdateUsernameRequest : IRequest<IResult<bool>>
    {
        [JsonIgnore] public string UserId { get; init; } = string.Empty;
        public string NewUsername { get; init; } = null!;
    }

    public class UpdateUsernameRequestHandler : IRequestHandler<UpdateUsernameRequest, IResult<bool>>
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

        public async Task<IResult<bool>> Handle(UpdateUsernameRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null) return Result<bool>.Fail(ResultError.Unauthorized);
            if (user.Id != _currentUserService.GetUserId())
                return Result<bool>.Fail(ResultError.Forbidden);

            // update username
            var result = await _identityService.UpdateUsername(user, request.NewUsername);
            return result.ToResult();
        }
    }
}