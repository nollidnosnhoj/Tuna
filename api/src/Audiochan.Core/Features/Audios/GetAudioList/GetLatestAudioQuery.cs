using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using MediatR;

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
            var audios = await _unitOfWork.Audios
                .GetLatestAudios(request, cancellationToken);
            
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