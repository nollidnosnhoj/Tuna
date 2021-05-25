using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Extensions.QueryableExtensions;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.GetAudioList
{
    public record GetAudioListRequest : IHasCursor, IRequest<CursorList<AudioViewModel>>
    {
        public string? Tag { get; init; }
        public string? Cursor { get; init; }
        public int Size { get; init; } = 30;
    }

    public class GetAudioListRequestHandler : IRequestHandler<GetAudioListRequest, CursorList<AudioViewModel>>
    {
        private readonly IAudioRepository _audioRepository;

        public GetAudioListRequestHandler(IAudioRepository audioRepository)
        {
            _audioRepository = audioRepository;
        }

        public async Task<CursorList<AudioViewModel>> Handle(GetAudioListRequest request,
            CancellationToken cancellationToken)
        {
            return await _audioRepository.GetCursorPaginationAsync(new GetAudioListSpecification(request.Size, request.Tag),
                request.Cursor, cancellationToken);
        }
    }
}