using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tuna.Application.Features.Users.Mappings;
using Tuna.Application.Features.Users.Models;
using Tuna.Application.Persistence;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Users.Queries;

public record GetUserQuery(long UserId) : IQueryRequest<UserDto?>;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto?>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public GetUserQueryHandler(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<UserDto?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Users
            .Where(x => x.Id == request.UserId)
            .ProjectToDto()
            .SingleOrDefaultAsync(cancellationToken);
    }
}