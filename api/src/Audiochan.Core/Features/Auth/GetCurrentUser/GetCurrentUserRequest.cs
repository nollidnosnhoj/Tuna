using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Auth.GetCurrentUser
{
    public record GetCurrentUserRequest : IRequest<CurrentUserViewModel?>
    {
    }

    public class GetCurrentUserRequestHandler : IRequestHandler<GetCurrentUserRequest, CurrentUserViewModel?>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public GetCurrentUserRequestHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CurrentUserViewModel?> Handle(GetCurrentUserRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            return await _unitOfWork.Users.GetBySpecAsync(new GetCurrentUserSpecification(currentUserId),
                cancellationToken: cancellationToken);
        }
    }
}