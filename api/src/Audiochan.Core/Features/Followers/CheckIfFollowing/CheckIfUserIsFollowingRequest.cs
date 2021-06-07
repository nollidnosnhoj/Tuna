using System.Linq;
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
        private readonly IUnitOfWork _unitOfWork;

        public CheckIfUserIsFollowingRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CheckIfUserIsFollowingRequest request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Users
                .Include(u => u.Followings)
                .AnyAsync(u => u.UserName == request.Username 
                               && u.Followings.Any(f => f.ObserverId == request.UserId), cancellationToken);
        }
    }
}