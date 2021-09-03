using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Audios.GetAudios
{
    public record GetAudiosQuery : IHasCursorPage<long>, IRequest<CursorPagedListDto<AudioDto, long>>
    {
        public List<string> Tags { get; init; } = new();
        public long Cursor { get; init; }
        public int Size { get; init; } = 30;
    }

    public sealed class GetAudiosSpecification : Specification<Audio, AudioDto>
    {
        public GetAudiosSpecification(GetAudiosQuery request)
        {
            Query.AsNoTracking();
            
            if (request.Tags.Count > 0)
            {
                Query.Where(a => a.Tags.Any(t => request.Tags.Contains(t.Name)));
            }

            Query.OrderByDescending(a => a.Id);
            Query.Select(AudioMaps.AudioToView());
        }
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
                .GetCursorPagedListAsync(spec, query.Cursor, query.Size, cancellationToken);
            return new CursorPagedListDto<AudioDto, long>(list, query.Size);
        }
    }
}