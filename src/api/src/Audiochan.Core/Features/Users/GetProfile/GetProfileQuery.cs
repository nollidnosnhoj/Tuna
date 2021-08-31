using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.GetProfile
{
    public record GetProfileQuery(string Username) : IRequest<ProfileDto?>
    {
    }

    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ProfileDto?>
    {
        private readonly ApplicationDbContext _unitOfWork;

        public GetProfileQueryHandler(ApplicationDbContext unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProfileDto?> Handle(GetProfileQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Users
                .AsNoTracking()
                .Where(u => u.UserName == query.Username)
                .Select(UserMaps.UserToProfileFunc)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}