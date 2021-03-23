using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Users.GetUserAudios
{
    public record GetUserAudiosQuery : AudioListQueryRequest
    {
        public string Username { get; init; }
    }

    public class GetUserAudiosQueryHandler : IRequestHandler<GetUserAudiosQuery, PagedList<AudioViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetUserAudiosQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<PagedList<AudioViewModel>> Handle(GetUserAudiosQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            return await _dbContext.Audios
                .DefaultListQueryable(currentUserId)
                .Where(a => a.User.UserName == request.Username.ToLower())
                .Sort(request.Sort)
                .Select(AudioMappingExtensions.AudioToListProjection())
                .PaginateAsync(request, cancellationToken);
        }
    }
}