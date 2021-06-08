using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.GetAudioList
{
    public record GetLatestAudioQuery : IRequest<GetAudioListViewModel>
    {
        public string? Tag { get; init; }
        public string? Cursor { get; init; }
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
                .Where(x => x.IsPublic);
            
            if (!string.IsNullOrWhiteSpace(request.Cursor))
            {
                var (since, id) = CursorHelpers.DecodeCursor(request.Cursor);
                if (Guid.TryParse(id, out var audioId) && since.HasValue)
                {
                    queryable = queryable.Where(a => a.Created < since.GetValueOrDefault() 
                                        || a.Created == since.GetValueOrDefault() && a.Id.CompareTo(audioId) < 0);
                }
            }

            var audios = await queryable
                .ProjectToList()
                .Take(request.Size)
                .ToListAsync(cancellationToken);
            
            var lastAudio = audios.LastOrDefault();

            var nextCursor = audios.Count < request.Size
                ? null
                : lastAudio != null
                    ? CursorHelpers.EncodeCursor(lastAudio.Uploaded, lastAudio.Id.ToString())
                    : null;

            return new GetAudioListViewModel(audios, nextCursor);
        }
    }
}