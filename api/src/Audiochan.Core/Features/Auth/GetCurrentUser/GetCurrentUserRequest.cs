using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Auth.GetCurrentUser
{
    public record GetCurrentUserRequest : IRequest<CurrentUserViewModel?>
    {
    }

    public class GetCurrentUserRequestHandler : IRequestHandler<GetCurrentUserRequest, CurrentUserViewModel?>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;

        public GetCurrentUserRequestHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext, IUserRepository userRepository)
        {
            _currentUserService = currentUserService;
            _dbContext = dbContext;
            _userRepository = userRepository;
        }

        public async Task<CurrentUserViewModel?> Handle(GetCurrentUserRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            return await _userRepository.GetBySpecAsync(new GetCurrentUserSpecification(currentUserId),
                cancellationToken: cancellationToken);
        }
    }
}