using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using MediatR;

namespace Audiochan.Core.Features.Users.GetUserFavoriteAudios
{
    public record GetUserFavoriteAudiosQuery : IHasOffsetPage, IRequest<OffsetPagedListDto<AudioDto>>
    {
        public string? Username { get; set; }
        public int Offset { get; init; }
        public int Size { get; init; }
    }
    
    public class GetUserFavoriteAudiosQueryHandler : IRequestHandler<GetUserFavoriteAudiosQuery, OffsetPagedListDto<AudioDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly long _currentUserId;

        public GetUserFavoriteAudiosQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<OffsetPagedListDto<AudioDto>> Handle(GetUserFavoriteAudiosQuery query, CancellationToken cancellationToken)
        {
            var results = await _unitOfWork.Audios.GetUserFavoriteAudios(query, cancellationToken);
            return new OffsetPagedListDto<AudioDto>(results, query.Offset, query.Size);
        }
    }
}