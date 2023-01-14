using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.CQRS;
using Audiochan.Core.Features.Users.Mappings;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.Queries.GetProfile
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