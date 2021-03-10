using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Followers.GetFollowings
{
    public record GetFollowingsQuery : PaginationQueryRequest<FollowingViewModel>
    {
        public string Username { get; init; }
    }

    public class GetFollowingsMappingProfile : Profile
    {
        public GetFollowingsMappingProfile()
        {
            CreateMap<FollowedUser, FollowingViewModel>()
                .ForMember(dest => dest.Username, opts =>
                    opts.MapFrom(src => src.Target.UserName))
                .ForMember(dest => dest.Picture, opts =>
                    opts.MapFrom(src => src.Target.Picture));
        }
    }

    public class GetFollowingsQueryHandler : IRequestHandler<GetFollowingsQuery, PagedList<FollowingViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetFollowingsQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PagedList<FollowingViewModel>> Handle(GetFollowingsQuery request,
            CancellationToken cancellationToken)
        {
            return await _dbContext.FollowedUsers
                .AsNoTracking()
                .Include(u => u.Target)
                .Include(u => u.Observer)
                .Where(u => u.Observer.UserName == request.Username.Trim().ToLower())
                .ProjectTo<FollowingViewModel>(_mapper.ConfigurationProvider)
                .PaginateAsync(request, cancellationToken);
        }
    }
}