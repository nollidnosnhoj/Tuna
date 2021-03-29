using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.GetUser
{
    public class GetUserRequestHandler : IRequestHandler<GetUserRequest, IResult<UserViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetUserRequestHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext)
        {
            _currentUserService = currentUserService;
            _dbContext = dbContext;
        }

        public async Task<IResult<UserViewModel>> Handle(GetUserRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            var profile = await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Followers)
                .Include(u => u.Followings)
                .Include(u => u.Audios)
                .Where(u => u.UserName == request.Username.Trim().ToLower())
                .ProjectToUser(currentUserId)
                .SingleOrDefaultAsync(cancellationToken);

            return profile == null
                ? Result<UserViewModel>.Fail(ResultError.NotFound)
                : Result<UserViewModel>.Success(profile);
        }
    }
}