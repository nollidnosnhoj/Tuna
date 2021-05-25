using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Followers.CheckIfFollowing
{
    public record CheckIfUserIsFollowingRequest(string UserId, string Username) : IRequest<bool>
    {
    }

    public class CheckIfUserIsFollowingRequestHandler : IRequestHandler<CheckIfUserIsFollowingRequest, bool>
    {
        private readonly IFollowedUserRepository _followedUserRepository;

        public CheckIfUserIsFollowingRequestHandler(IFollowedUserRepository followedUserRepository)
        {
            _followedUserRepository = followedUserRepository;
        }

        public async Task<bool> Handle(CheckIfUserIsFollowingRequest request, CancellationToken cancellationToken)
        {
            return await _followedUserRepository.ExistsAsync(
                new CheckIfUserIsFollowingSpecification(request.Username, request.UserId), cancellationToken: cancellationToken);
        }
    }
}