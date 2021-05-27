using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Audios.GetAudioList
{
    public record GetAudioListRequest : IHasCursor, IRequest<CursorListDto<AudioViewModel>>
    {
        public string? Tag { get; init; }
        public string? Cursor { get; init; }
        public int Size { get; init; } = 30;
    }

    public class GetAudioListRequestHandler : IRequestHandler<GetAudioListRequest, CursorListDto<AudioViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAudioListRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CursorListDto<AudioViewModel>> Handle(GetAudioListRequest request,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.Audios.GetCursorPaginationAsync(
                new GetAudioListSpecification(request.Tag), request.Cursor, request.Size, cancellationToken);
        }
    }
}