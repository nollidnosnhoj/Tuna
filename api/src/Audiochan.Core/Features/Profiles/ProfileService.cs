using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Profiles.Models;
using Audiochan.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Profiles
{
    public class ProfileService : IProfileService
    {
        private readonly IAudiochanContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public ProfileService(IAudiochanContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<IResult<ProfileViewModel>> GetProfile(string username, 
            CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();
            
            var profile = await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Followers)
                .Include(u => u.Followings)
                .Include(u => u.Audios)
                .Include(u => u.Profile)
                .Where(u => u.UserName == username)
                .Select(MapProjections.Profile(currentUserId))
                .SingleOrDefaultAsync(cancellationToken);

            return profile == null
                ? Result<ProfileViewModel>.Fail(ResultErrorCode.NotFound)
                : Result<ProfileViewModel>.Success(profile);
        }
    }
}