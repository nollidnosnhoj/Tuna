using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Audios.GetAudioList;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models;
using Audiochan.Core.Persistence;
using MediatR;

namespace Audiochan.API.Features.Audios.GetAudioFeed
{
    public record GetAudioFeedRequest : IHasPage, IRequest<PagedListDto<AudioViewModel>>
    {
        public string UserId { get; init; } = string.Empty;
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class GetAudioFeedRequestHandler : IRequestHandler<GetAudioFeedRequest, PagedListDto<AudioViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAudioFeedRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedListDto<AudioViewModel>> Handle(GetAudioFeedRequest request,
            CancellationToken cancellationToken)
        {
            var followedIds = await _unitOfWork.FollowedUsers
                .GetListAsync(new GetFollowingIdsSpecification(request.UserId), cancellationToken: cancellationToken);

            return await _unitOfWork.Audios
                .GetPagedListBySpec(new GetAudioFeedSpecification(followedIds), request.Page, request.Size, cancellationToken: cancellationToken);
        }
    }
}