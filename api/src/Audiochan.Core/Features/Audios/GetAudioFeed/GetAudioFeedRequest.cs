using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Audios.GetAudioFeed
{
    public record GetAudioFeedRequest : IRequest<CursorList<AudioViewModel>>
    {
        public string UserId { get; init; } = string.Empty;
        public string? Cursor { get; init; }
    }

    public class GetAudioFeedRequestHandler : IRequestHandler<GetAudioFeedRequest, CursorList<AudioViewModel>>
    {
        private readonly IAudioRepository _audioRepository;
        private readonly IFollowedUserRepository _followedUserRepository;

        public GetAudioFeedRequestHandler(IFollowedUserRepository followedUserRepository, IAudioRepository audioRepository)
        {
            _followedUserRepository = followedUserRepository;
            _audioRepository = audioRepository;
        }

        public async Task<CursorList<AudioViewModel>> Handle(GetAudioFeedRequest request,
            CancellationToken cancellationToken)
        {
            var followedIds = await _followedUserRepository
                .GetListBySpecAsync(new GetFollowingIdsSpecification(request.UserId), cancellationToken: cancellationToken);

            return await _audioRepository.GetCursorPaginationAsync(new GetAudioFeedSpecification(followedIds), request.Cursor, cancellationToken);

            // return await _dbContext.Audios
            //     .AsNoTracking()
            //     .Include(x => x.User)
            //     .ExcludePrivateAudios()
            //     .Where(a => followedIds.Contains(a.UserId))
            //     .ProjectToList()
            //     .OrderByDescending(a => a.Uploaded)
            //     .PaginateAsync(cancellationToken: cancellationToken);
        }
    }
}