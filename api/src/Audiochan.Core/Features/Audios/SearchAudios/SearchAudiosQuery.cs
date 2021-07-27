using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetLatestAudios;
using MediatR;

namespace Audiochan.Core.Features.Audios.SearchAudios
{
    public record SearchAudiosQuery : IHasPage, IRequest<PagedListDto<AudioViewModel>>
    {
        public string Q { get; init; } = string.Empty;
        public string Tags { get; init; } = string.Empty;
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class SearchAudiosQueryHandler : IRequestHandler<SearchAudiosQuery, PagedListDto<AudioViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchAudiosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedListDto<AudioViewModel>> Handle(SearchAudiosQuery query,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.Audios.SearchAudios(query, cancellationToken);
        }
    }
}