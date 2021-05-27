using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Shared.Helpers;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.API.Features.Audios.GetAudioList
{
    public record GetAudioListRequest : IRequest<GetAudioListViewModel>
    {
        public string? Tag { get; init; }
        public string? Cursor { get; init; }
        public int Size { get; init; } = 30;
    }

    public class GetAudioListRequestHandler : IRequestHandler<GetAudioListRequest, GetAudioListViewModel>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAudioListRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetAudioListViewModel> Handle(GetAudioListRequest request,
            CancellationToken cancellationToken)
        {
            var spec = new GetAudioListSpecification(request.Cursor, request.Size, request.Tag);
            var audios = await _unitOfWork.Audios.GetListAsync(spec, 
                cancellationToken: cancellationToken);
            
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