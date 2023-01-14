using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Auth.Mappings;
using Audiochan.Core.CQRS;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Auth.Queries
{
    public record GetCurrentUserQuery(long UserId) : IQueryRequest<CurrentUserDto?>
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
                .Where(u => u.Id == query.UserId)
                .Project()
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}