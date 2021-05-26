using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Users.UpdateEmail
{
    public record UpdateEmailRequest : IRequest<IResult<bool>>
    {
        [JsonIgnore] public string UserId { get; init; } = string.Empty;
        public string NewEmail { get; init; } = null!;
    }


    public class UpdateEmailRequestHandler : IRequestHandler<UpdateEmailRequest, IResult<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;

        public UpdateEmailRequestHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, 
            IIdentityService identityService)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        public async Task<IResult<bool>> Handle(UpdateEmailRequest request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null) return Result<bool>.Fail(ResultError.Unauthorized);
            if (user.Id != _currentUserService.GetUserId())
                return Result<bool>.Fail(ResultError.Forbidden);

            var result = await _identityService.UpdateEmail(user, request.NewEmail);
            return result.ToResult();
        }
    }
}