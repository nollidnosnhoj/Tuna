using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios;
using MediatR;

namespace Audiochan.Core.Features.Users.GetUserAudios
{
    public class GetUserAudiosRequest : IHasCursor, IRequest<CursorList<AudioViewModel>>
    {
        public string? Username { get; set; }
        public string? Cursor { get; init; }
        public int Size { get; init; } = 30;
    }

    public class GetUserAudiosRequestHandler : IRequestHandler<GetUserAudiosRequest, CursorList<AudioViewModel>>
    {
        private readonly IAudioRepository _audioRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetUserAudiosRequestHandler(ICurrentUserService currentUserService, IAudioRepository audioRepository)
        {
            _currentUserService = currentUserService;
            _audioRepository = audioRepository;
        }

        public async Task<CursorList<AudioViewModel>> Handle(GetUserAudiosRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            return await _audioRepository.GetCursorPaginationAsync(
                new GetUserAudiosSpecification(request.Username, currentUserId, request.Size), 
                request.Cursor, cancellationToken);
        }
    }
}