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
        private readonly IUnitOfWork _unitOfWork;

        public GetAudioFeedRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CursorList<AudioViewModel>> Handle(GetAudioFeedRequest request,
            CancellationToken cancellationToken)
        {
            var followedIds = await _unitOfWork.FollowedUsers
                .GetListBySpecAsync(new GetFollowingIdsSpecification(request.UserId), cancellationToken: cancellationToken);

            return await _unitOfWork.Audios
                .GetCursorPaginationAsync(new GetAudioFeedSpecification(followedIds), request.Cursor, cancellationToken);
        }
    }
}