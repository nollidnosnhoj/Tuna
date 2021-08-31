using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Users;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Auth.GetCurrentUser
{
    public record GetCurrentUserQuery(long UserId) : IRequest<CurrentUserDto?>
    {
    }

    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDto?>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetCurrentUserQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CurrentUserDto?> Handle(GetCurrentUserQuery query,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == query.UserId)
                .Select(UserMaps.UserToCurrentUserFunc)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}