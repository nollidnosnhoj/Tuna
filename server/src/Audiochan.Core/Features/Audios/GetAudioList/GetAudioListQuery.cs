using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Audios.GetAudioList
{
    public record GetAudioListQuery : AudioListQueryRequest
    {
    }

    public class GetAudioListQueryHandler : IRequestHandler<GetAudioListQuery, PagedList<AudioViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetAudioListQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<PagedList<AudioViewModel>> Handle(GetAudioListQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            return await _dbContext.Audios
                .DefaultListQueryable(currentUserId)
                .Sort(request.Sort)
                .Select(AudioMappingExtensions.AudioToListProjection())
                .PaginateAsync(request, cancellationToken);
        }
    }
}