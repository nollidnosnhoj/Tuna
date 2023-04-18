using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tuna.Application.Features.Audios.Mappings;
using Tuna.Application.Features.Audios.Models;
using Tuna.Application.Persistence;
using Tuna.Shared.Mediatr;
using Tuna.Shared.Models;

namespace Tuna.Application.Features.Audios.Queries;

public record GetUserAudiosQuery(long UserId, int? Skip, int? Take)
    : OffsetPagedQuery(Skip, Take), IQueryRequest<CollectionSegment<AudioDto>>;

public class GetUserAudiosQueryHandler : IRequestHandler<GetUserAudiosQuery, CollectionSegment<AudioDto>>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public GetUserAudiosQueryHandler(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<CollectionSegment<AudioDto>> Handle(GetUserAudiosQuery request,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Audios
            .Where(x => x.UserId == request.UserId)
            .OrderByDescending(x => x.CreatedAt)
            .ProjectToDto()
            .ApplyOffsetPaginationAsync(request.Skip, request.Take, cancellationToken);
    }
}