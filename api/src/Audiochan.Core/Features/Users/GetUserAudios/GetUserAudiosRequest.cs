using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Users.GetUserAudios
{
    public class GetUserAudiosRequest : IHasCursor, IRequest<CursorListDto<AudioViewModel>>
    {
        public string? Username { get; set; }
        public string? Cursor { get; init; }
        public int Size { get; init; } = 30;
    }

    public class GetUserAudiosRequestHandler : IRequestHandler<GetUserAudiosRequest, CursorListDto<AudioViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetUserAudiosRequestHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CursorListDto<AudioViewModel>> Handle(GetUserAudiosRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            return await _unitOfWork.Audios.GetCursorPaginationAsync(
                new GetUserAudiosSpecification(request.Username, currentUserId), 
                request.Cursor, request.Size, cancellationToken);
        }
    }
}