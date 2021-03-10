using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.GetCurrentUser
{
    public record GetCurrentUserQuery : IRequest<IResult<CurrentUserViewModel>>
    {
    }

    public class CurrentUserMappingProfile : Profile
    {
        public CurrentUserMappingProfile()
        {
            CreateMap<User, CurrentUserViewModel>();
        }
    }

    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, IResult<CurrentUserViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetCurrentUserQueryHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext,
            IMapper mapper)
        {
            _currentUserService = currentUserService;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IResult<CurrentUserViewModel>> Handle(GetCurrentUserQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            var user = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == currentUserId)
                .ProjectTo<CurrentUserViewModel>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);

            return user == null
                ? Result<CurrentUserViewModel>.Fail(ResultError.Unauthorized)
                : Result<CurrentUserViewModel>.Success(user);
        }
    }
}