using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Entities.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.GetAudioList
{
    public record GetLatestAudioQuery : IRequest<GetAudioListViewModel>
    {
        public string? Tag { get; init; }
        public long? Cursor { get; init; }
        public int Size { get; init; } = 30;
    }

    public class GetLatestAudioQueryHandler : IRequestHandler<GetLatestAudioQuery, GetAudioListViewModel>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLatestAudioQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetAudioListViewModel> Handle(GetLatestAudioQuery request,
            CancellationToken cancellationToken)
        {
            var queryable = _unitOfWork.Audios
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.Visibility == Visibility.Public);
            
            if (request.Cursor is not null)
                queryable = queryable.Where(x => x.Id < request.Cursor.Value);

            var audios = await queryable
                .ProjectToList()
                .Take(request.Size)
                .ToListAsync(cancellationToken);
            
            var lastAudio = audios.LastOrDefault();

            long? nextCursor = audios.Count < request.Size
                ? null
                : lastAudio != null
                    ? lastAudio.Id
                    : null;

            return new GetAudioListViewModel(audios, nextCursor);
        }
    }
}