using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Audios.Mappings;
using Audiochan.Core.CQRS;
using Audiochan.Core.Dtos;
using Audiochan.Core.Extensions;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Audios.Queries
{
    public record GetAudioQuery(long Id) : IQueryRequest<AudioDto?>
    {
    }

    public class GetAudioQueryHandler : IRequestHandler<GetAudioQuery, AudioDto?>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetAudioQueryHandler(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<AudioDto?> Handle(GetAudioQuery query, CancellationToken cancellationToken)
        {
            _currentUserService.User.TryGetUserId(out var userId);
            var user = await _dbContext.Audios
                .Where(a => a.Id == query.Id)
                .Project(userId)
                .SingleOrDefaultAsync(cancellationToken);
            return user?.Map();
        }
    }
}