using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly IMapper _mapper;

        public GetUserAudiosQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<PagedList<AudioViewModel>> Handle(GetUserAudiosQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            return await _dbContext.Audios
                .DefaultQueryable(currentUserId)
                .Where(a => a.User.UserName == request.Username.ToLower())
                .FilterByGenre(request.Genre)
                .Sort(request.Sort)
                .ProjectTo<AudioViewModel>(_mapper.ConfigurationProvider, new {currentUserId})
                .PaginateAsync(request, cancellationToken);
        }
    }
}