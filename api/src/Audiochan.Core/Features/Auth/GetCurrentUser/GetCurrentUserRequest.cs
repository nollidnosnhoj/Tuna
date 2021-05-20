using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Auth.GetCurrentUser
{
    public record GetCurrentUserRequest : IRequest<CurrentUserViewModel>
    {
    }

    public class GetCurrentUserRequestHandler : IRequestHandler<GetCurrentUserRequest, CurrentUserViewModel>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetCurrentUserRequestHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext)
        {
            _currentUserService = currentUserService;
            _dbContext = dbContext;
        }

        public async Task<CurrentUserViewModel> Handle(GetCurrentUserRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            return await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == currentUserId)
                .ProjectToCurrentUser()
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}