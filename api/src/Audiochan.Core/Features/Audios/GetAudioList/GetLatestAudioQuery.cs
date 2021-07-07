using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Interfaces;
using MediatR;

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
            var audios = await _unitOfWork.Audios
                .GetLatestAudios(request, cancellationToken);

            var nextCursor = GetNextCursor(audios, request.Size);

            return new GetAudioListViewModel(audios, nextCursor);
        }

        private string? GetNextCursor(List<AudioViewModel> audios, int size)
        {
            var lastAudio = audios.LastOrDefault();

            return audios.Count < size
                ? null
                : lastAudio != null
                    ? CursorHelpers.Encode(lastAudio.Id, lastAudio.Created)
                    : null;
        }
    }
}