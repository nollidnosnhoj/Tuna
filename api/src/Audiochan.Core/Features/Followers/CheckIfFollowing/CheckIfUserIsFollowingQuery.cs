using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Followers.CheckIfFollowing
{
    public record CheckIfUserIsFollowingQuery(string UserId, string Username) : IRequest<bool>
    {
    }

    public class CheckIfUserIsFollowingQueryHandler : IRequestHandler<CheckIfUserIsFollowingQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckIfUserIsFollowingQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CheckIfUserIsFollowingQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Users
                .Include(u => u.Followings)
                .AnyAsync(u => u.UserName == query.Username 
                               && u.Followings.Any(f => f.ObserverId == query.UserId), cancellationToken);
        }
    }
}