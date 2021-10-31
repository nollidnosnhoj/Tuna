using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Models.Pagination;
using MediatR;

namespace Audiochan.Core.Audios.Queries
{
    public record GetAudioFeedQuery(long UserId) : IHasOffsetPage, IRequest<OffsetPagedListDto<AudioDto>>
    {
        public int Offset { get; init; }
        public int Size { get; init; }
    }

    public class GetAudioFeedQueryHandler : IRequestHandler<GetAudioFeedQuery, OffsetPagedListDto<AudioDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAudioFeedQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OffsetPagedListDto<AudioDto>> Handle(GetAudioFeedQuery query,
            CancellationToken cancellationToken)
        {
            var followingIds = await _unitOfWork.Users.GetObserverFollowingIds(query.UserId, cancellationToken);
            var spec = new GetAudioFeedSpecification(followingIds);
            var list = await _unitOfWork.Audios
                .GetOffsetPagedListAsync<AudioDto>(spec, query.Offset, query.Size, cancellationToken);
            return new OffsetPagedListDto<AudioDto>(list, query.Offset, query.Size);
        }
    }
}