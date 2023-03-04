using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Shared.Mediatr;
using Audiochan.Shared.Models;
using Audiochan.Core.Features.Audios.Mappings;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Persistence;
using HotChocolate.Types.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.Queries;

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
    
    public async Task<CollectionSegment<AudioDto>> Handle(GetUserFavoriteAudiosQuery request, CancellationToken cancellationToken)
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