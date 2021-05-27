using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.API.Features.Followers.CheckIfFollowing
{
    public record CheckIfUserIsFollowingRequest(string UserId, string Username) : IRequest<bool>
    {
    }

    public class CheckIfUserIsFollowingRequestHandler : IRequestHandler<CheckIfUserIsFollowingRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckIfUserIsFollowingRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CheckIfUserIsFollowingRequest request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.FollowedUsers.ExistsAsync(
                new CheckIfUserIsFollowingSpecification(request.Username, request.UserId), 
                cancellationToken: cancellationToken);
        }
    }
}