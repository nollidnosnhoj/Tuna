using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Models.Pagination;
using MediatR;

namespace Audiochan.Core.Audios.Queries
{
    public record GetAudiosQuery : IHasCursorPage<long>, IRequest<CursorPagedListDto<AudioDto, long>>
    {
        public List<string> Tags { get; init; } = new();
        public long Cursor { get; init; }
        public int Size { get; init; } = 30;
    }

    public class GetAudiosQueryHandler : IRequestHandler<GetAudiosQuery, CursorPagedListDto<AudioDto, long>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAudiosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CursorPagedListDto<AudioDto, long>> Handle(GetAudiosQuery query,
            CancellationToken cancellationToken)
        {
            var spec = new GetAudiosSpecification(query);
            var list = await _unitOfWork.Audios
                .GetCursorPagedListAsync<AudioDto, long>(spec, query.Cursor, query.Size, cancellationToken);
            return new CursorPagedListDto<AudioDto, long>(list, query.Size);
        }
    }
}