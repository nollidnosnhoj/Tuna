using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Extensions.QueryableExtensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.GetAudioFeed
{
    public record GetAudioFeedRequest : IRequest<PagedList<AudioViewModel>>
    {
        public string UserId { get; init; }
    }
    
    public class GetAudioFeedRequestHandler : IRequestHandler<GetAudioFeedRequest, PagedList<AudioViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly MediaStorageSettings _storageSettings;

        public GetAudioFeedRequestHandler(IApplicationDbContext dbContext, IOptions<MediaStorageSettings> options)
        {
            _dbContext = dbContext;
            _storageSettings = options.Value;
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
                .BaseListQueryable(request.UserId)
                .Where(a => followedIds.Contains(a.UserId))
                .ProjectToList(_storageSettings)
                .OrderByDescending(a => a.Uploaded)
                .PaginateAsync(cancellationToken: cancellationToken);
        }
    }

}