using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Common.Models;
using Audiochan.Core.Features.Audios.Mappings;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Persistence;
using HotChocolate.Types.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.Queries;

public record GetUserAudiosQuery(long UserId, int? Skip, int? Take) 
    : OffsetPagedQuery(Skip, Take), IQueryRequest<CollectionSegment<AudioViewModel>>;

public class GetUserAudiosQueryHandler : IRequestHandler<GetUserAudiosQuery, CollectionSegment<AudioViewModel>>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public GetUserAudiosQueryHandler(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    public async Task<CollectionSegment<AudioViewModel>> Handle(GetUserAudiosQuery request, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Audios
            .Where(x => x.UserId == request.UserId)
            .OrderByDescending(x => x.CreatedAt)
            .Project()
            .ApplyOffsetPaginationAsync(request.Skip, request.Take, cancellationToken);
    }
}