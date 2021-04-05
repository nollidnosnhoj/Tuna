using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.GetAudioFeed
{
    public class GetAudioFeedRequestHandler : IRequestHandler<GetAudioFeedRequest, PagedList<AudioViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly AudiochanOptions _audiochanOptions;

        public GetAudioFeedRequestHandler(IApplicationDbContext dbContext, IOptions<AudiochanOptions> options)
        {
            _dbContext = dbContext;
            _audiochanOptions = options.Value;
        }

        public async Task<PagedList<AudioViewModel>> Handle(GetAudioFeedRequest request,
            CancellationToken cancellationToken)
        {
            var followedIds = await _dbContext.FollowedUsers
                .AsNoTracking()
                .Where(user => user.ObserverId == request.UserId)
                .Select(user => user.TargetId)
                .ToListAsync(cancellationToken);

            return await _dbContext.Audios
                .DefaultListQueryable(request.UserId)
                .Where(a => followedIds.Contains(a.UserId))
                .ProjectToList(_audiochanOptions)
                .OrderByDescending(a => a.Uploaded)
                .PaginateAsync(cancellationToken: cancellationToken);
        }
    }
}