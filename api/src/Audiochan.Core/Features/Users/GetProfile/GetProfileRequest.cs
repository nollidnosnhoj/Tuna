using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.GetProfile
{
    public record GetProfileRequest(string Username) : IRequest<ProfileViewModel?>
    {
    }

    public class GetProfileRequestHandler : IRequestHandler<GetProfileRequest, ProfileViewModel?>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public GetProfileRequestHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProfileViewModel?> Handle(GetProfileRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            return await _unitOfWork.Users.AsNoTracking()
                .Include(u => u.Followers)
                .Include(u => u.Followings)
                .Include(u => u.Audios)
                .Where(u => u.UserName == request.Username.Trim().ToLower())
                .ProjectToUser(currentUserId)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}