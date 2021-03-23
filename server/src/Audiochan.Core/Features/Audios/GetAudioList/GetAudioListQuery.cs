using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.GetAudioList
{
    public record GetAudioListQuery : AudioListQueryRequest
    {
    }

    public class GetAudioListQueryHandler : IRequestHandler<GetAudioListQuery, PagedList<AudioViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly AudiochanOptions _audiochanOptions;

        public GetAudioListQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IOptions<AudiochanOptions> options)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _audiochanOptions = options.Value;
        }

        public async Task<PagedList<AudioViewModel>> Handle(GetAudioListQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            return await _dbContext.Audios
                .DefaultListQueryable(currentUserId)
                .Sort(request.Sort)
                .Select(AudioMappingExtensions.AudioToListProjection(_audiochanOptions))
                .PaginateAsync(request, cancellationToken);
        }
    }
}