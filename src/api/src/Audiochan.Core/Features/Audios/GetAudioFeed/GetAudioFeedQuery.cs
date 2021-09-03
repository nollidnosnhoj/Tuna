using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Audios.GetAudioFeed
{
    public record GetAudioFeedQuery : IHasOffsetPage, IRequest<OffsetPagedListDto<AudioDto>>
    {
        public long UserId { get; init; }
        public int Offset { get; init; }
        public int Size { get; init; }
    }

    public sealed class GetAudioFeedSpecification : Specification<Audio, AudioDto>
    {
        public GetAudioFeedSpecification(long[] userIds)
        {
            Query.AsNoTracking();
            Query.Where(a => userIds.Contains(a.UserId));
            Query.OrderByDescending(a => a.Created);
            Query.Select(AudioMaps.AudioToView());
        }
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
            var list = await _unitOfWork.Audios.GetOffsetPagedListAsync(spec, query.Offset, query.Size, cancellationToken);
            return new OffsetPagedListDto<AudioDto>(list, query.Offset, query.Size);
        }
    }
}