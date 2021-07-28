using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.GetProfile
{
    public record GetProfileQuery(string Username) : IRequest<ProfileViewModel?>
    {
    }

    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ProfileViewModel?>
    {
        private readonly ApplicationDbContext _unitOfWork;

        public GetProfileQueryHandler(ApplicationDbContext unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProfileViewModel?> Handle(GetProfileQuery query, CancellationToken cancellationToken)
        {
            var userId = await _unitOfWork.Users.AsNoTracking()
                .Select(u => u.Id)
                .SingleOrDefaultAsync(cancellationToken);
            
            if (string.IsNullOrEmpty(userId)) return null;
            
            return await _unitOfWork.Users.AsNoTracking()
                .AsSplitQuery()
                .Include(u => u.Followers.Where(fu => fu.TargetId == userId))
                .Include(u => u.Followings.Where(fu => fu.ObserverId == userId))
                .Include(u => u.Audios)
                .Where(u => u.Id == userId)
                .Select(UserMaps.UserToProfileFunc)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}