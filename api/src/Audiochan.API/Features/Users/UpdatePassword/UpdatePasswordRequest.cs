using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.API.Features.Users.UpdatePassword
{
    public record UpdatePasswordRequest : IRequest<Result<bool>>
    {
        [JsonIgnore] public string UserId { get; init; } = string.Empty;
        public string CurrentPassword { get; init; } = "";
        public string NewPassword { get; init; } = "";
    }

    public class UpdatePasswordRequestHandler : IRequestHandler<UpdatePasswordRequest, Result<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;

        public UpdatePasswordRequestHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, 
            IIdentityService identityService)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        public async Task<Result<bool>> Handle(UpdatePasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(new object[]{request.UserId}, cancellationToken);            
            if (user == null) return Result<bool>.Fail(ResultError.Unauthorized);
            if (user.Id != _currentUserService.GetUserId())
                return Result<bool>.Fail(ResultError.Forbidden);

            return await _identityService.UpdatePassword(user, request.CurrentPassword, request.NewPassword);
        }
    }
}