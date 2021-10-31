using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Audios;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Models.Pagination;
using MediatR;

namespace Audiochan.Core.Users
{
    public record GetUsersAudioQuery(string Username) : IHasOffsetPage, IRequest<OffsetPagedListDto<AudioDto>>
    {
        public int Offset { get; init; } = 1;
        public int Size { get; init; } = 30;
    }

    public class GetUsersAudioQueryHandler : IRequestHandler<GetUsersAudioQuery, OffsetPagedListDto<AudioDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUsersAudioQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OffsetPagedListDto<AudioDto>> Handle(GetUsersAudioQuery request,
            CancellationToken cancellationToken)
        {
            var spec = new GetUserAudiosSpecification(request.Username);
            var list = await _unitOfWork.Audios
                .GetOffsetPagedListAsync<AudioDto>(spec, request.Offset, request.Size, cancellationToken);
            return new OffsetPagedListDto<AudioDto>(list, request.Offset, request.Size);
        }
    }
}