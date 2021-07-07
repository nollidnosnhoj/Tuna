using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Users.UpdateEmail
{
    public record UpdateEmailCommand : IRequest<Result<bool>>
    {
        public string UserId { get; init; } = null!;
        public string NewEmail { get; init; } = null!;

        public static UpdateEmailCommand FromRequest(string userId, UpdateEmailRequest request) => new()
        {
            UserId = userId,
            NewEmail = request.NewEmail
        };
    }


    public class UpdateEmailCommandHandler : IRequestHandler<UpdateEmailCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;

        public UpdateEmailCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, 
            IIdentityService identityService)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        public async Task<Result<bool>> Handle(UpdateEmailCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.LoadAsync(new object[]{command.UserId}, cancellationToken);
            if (user == null) return Result<bool>.Unauthorized();
            if (user.Id != _currentUserService.GetUserId())
                return Result<bool>.Forbidden();

            return await _identityService.UpdateEmail(user, command.NewEmail);
        }
    }
}