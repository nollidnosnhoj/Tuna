using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.GetCurrentUser
{
    public class GetCurrentUserRequestHandler : IRequestHandler<GetCurrentUserRequest, IResult<CurrentUserViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetCurrentUserRequestHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext)
        {
            _currentUserService = currentUserService;
            _dbContext = dbContext;
        }

        public async Task<IResult<CurrentUserViewModel>> Handle(GetCurrentUserRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            var user = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == currentUserId)
                .ProjectToCurrentUser()
                .SingleOrDefaultAsync(cancellationToken);

            return user == null
                ? Result<CurrentUserViewModel>.Fail(ResultError.Unauthorized)
                : Result<CurrentUserViewModel>.Success(user);
        }
    }
}