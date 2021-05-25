using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Users.GetProfile
{
    public record GetProfileRequest(string Username) : IRequest<ProfileViewModel?>
    {
    }

    public class GetProfileRequestHandler : IRequestHandler<GetProfileRequest, ProfileViewModel?>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;

        public GetProfileRequestHandler(ICurrentUserService currentUserService, IUserRepository userRepository)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }

        public async Task<ProfileViewModel?> Handle(GetProfileRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            return await _userRepository.GetBySpecAsync(new GetProfileSpecification(request.Username, currentUserId),
                cancellationToken: cancellationToken);
        }
    }
}