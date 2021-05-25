using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Users.UpdatePassword
{
    public record UpdatePasswordRequest : IRequest<IResult<bool>>
    {
        [JsonIgnore] public string UserId { get; init; } = string.Empty;
        public string CurrentPassword { get; init; } = "";
        public string NewPassword { get; init; } = "";
    }

    public class UpdatePasswordRequestHandler : IRequestHandler<UpdatePasswordRequest, IResult<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;

        public UpdatePasswordRequestHandler(ICurrentUserService currentUserService, IUserRepository userRepository, 
            IIdentityService identityService)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _identityService = identityService;
        }

        public async Task<IResult<bool>> Handle(UpdatePasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);            
            if (user == null) return Result<bool>.Fail(ResultError.Unauthorized);
            if (user.Id != _currentUserService.GetUserId())
                return Result<bool>.Fail(ResultError.Forbidden);

            var result = await _identityService.UpdatePassword(user, request.CurrentPassword, request.NewPassword);
            return result.ToResult();
        }
    }
}