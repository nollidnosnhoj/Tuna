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

public record GetUserFavoriteAudiosQuery(long UserId, int? Skip, int? Take)
    : OffsetPagedQuery(Skip, Take), IQueryRequest<CollectionSegment<AudioDto>>;

public class GetUserFavoriteAudiosQueryHandler
    : IRequestHandler<GetUserFavoriteAudiosQuery, CollectionSegment<AudioDto>>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public GetUserFavoriteAudiosQueryHandler(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<CollectionSegment<AudioDto>> Handle(GetUserFavoriteAudiosQuery request,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.FavoriteAudios
            .Where(x => x.UserId == request.UserId)
            .Select(x => x.Audio)
            .OrderByDescending(x => x.CreatedAt)
            .ProjectToDto()
            .ApplyOffsetPaginationAsync(request.Skip, request.Take, cancellationToken);
    }
}