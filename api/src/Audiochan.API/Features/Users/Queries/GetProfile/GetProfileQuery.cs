using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Users.Mappings;
using Audiochan.Core.CQRS;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Users.Queries
{
    public record GetProfileQuery(string Username) : IQueryRequest<ProfileDto?>
    {
    }

    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ProfileDto?>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetProfileQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProfileDto?> Handle(GetProfileQuery query, CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .Where(u => u.UserName == query.Username)
                .ProjectToProfile()
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}