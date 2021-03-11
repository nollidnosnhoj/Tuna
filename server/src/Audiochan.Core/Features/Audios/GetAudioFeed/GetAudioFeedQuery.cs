using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.GetAudioFeed
{
    public record GetAudioFeedQuery : IRequest<PagedList<AudioViewModel>>
    {
        public string UserId { get; init; }
    }

    public class GetAudioFeedQueryHandler : IRequestHandler<GetAudioFeedQuery, PagedList<AudioViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetAudioFeedQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PagedList<AudioViewModel>> Handle(GetAudioFeedQuery request,
            CancellationToken cancellationToken)
        {
            var followedIds = await _dbContext.FollowedUsers
                .AsNoTracking()
                .Where(user => user.ObserverId == request.UserId)
                .Select(user => user.TargetId)
                .ToListAsync(cancellationToken);

            return await _dbContext.Audios
                .DefaultQueryable(request.UserId)
                .Where(a => followedIds.Contains(a.UserId))
                .ProjectTo<AudioViewModel>(_mapper.ConfigurationProvider, new {currentUserId = request.UserId})
                .OrderByDescending(a => a.Created)
                .PaginateAsync(cancellationToken);
        }
    }
}