using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Core.Features.Users.Mappings;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users;

public record GetUserQuery(long UserId) : IQueryRequest<UserViewModel?>;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserViewModel?>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public GetUserQueryHandler(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<UserViewModel?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Users
            .Where(x => x.Id == request.UserId)
            .ProjectToUser()
            .SingleOrDefaultAsync(cancellationToken);
    }
}