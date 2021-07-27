using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetLatestAudios;
using MediatR;

namespace Audiochan.Core.Features.Audios.GetAudioFeed
{
    public record GetAudioFeedQuery : IHasPage, IRequest<PagedListDto<AudioViewModel>>
    {
        public string UserId { get; init; } = string.Empty;
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class GetAudioFeedQueryHandler : IRequestHandler<GetAudioFeedQuery, PagedListDto<AudioViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAudioFeedQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedListDto<AudioViewModel>> Handle(GetAudioFeedQuery query,
            CancellationToken cancellationToken)
        {
            var ids = await _unitOfWork.Users.GetFollowingIds(query.UserId, cancellationToken);
            return await _unitOfWork.Audios.GetFollowedAudios(ids, cancellationToken);
        }
    }
}