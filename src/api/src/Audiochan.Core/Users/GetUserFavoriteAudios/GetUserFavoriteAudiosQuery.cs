using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Audios;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Models.Pagination;
using MediatR;

namespace Audiochan.Core.Users
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

        public GetUserFavoriteAudiosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OffsetPagedListDto<AudioDto>> Handle(GetUserFavoriteAudiosQuery query, CancellationToken cancellationToken)
        {
            var results = await _unitOfWork.Audios.GetUserFavoriteAudios(query, cancellationToken);
            return new OffsetPagedListDto<AudioDto>(results, query.Offset, query.Size);
        }
    }
}