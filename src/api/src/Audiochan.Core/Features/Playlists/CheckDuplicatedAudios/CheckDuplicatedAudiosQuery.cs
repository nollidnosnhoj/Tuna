using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Playlists.CheckDuplicatedAudios
{
    public record CheckDuplicatedAudiosQuery(Guid PlaylistId, List<Guid> AudioIds) : IRequest<List<Guid>>;
    
    public class CheckDuplicatedAudiosQueryHandler 
        : IRequestHandler<CheckDuplicatedAudiosQuery, List<Guid>>
    {
        private readonly ApplicationDbContext _dbContext;

        public CheckDuplicatedAudiosQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Guid>> Handle(CheckDuplicatedAudiosQuery request, 
            CancellationToken cancellationToken)
        {
            return await _dbContext.PlaylistAudios
                .Where(pa => pa.PlaylistId == request.PlaylistId)
                .Where(pa => request.AudioIds.Contains(pa.AudioId))
                .Select(pa => pa.AudioId)
                .ToListAsync(cancellationToken);
        }
    }
}