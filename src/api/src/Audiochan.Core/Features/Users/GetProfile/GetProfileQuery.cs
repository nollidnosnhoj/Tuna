using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Persistence;
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
                .Where(u => u.UserName == query.Username)
                .Select(u => u.Id)
                .SingleOrDefaultAsync(cancellationToken);
            
            if (!UserHelpers.IsValidId(userId)) return null;
            
            return await _unitOfWork.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(UserMaps.UserToProfileFunc)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}