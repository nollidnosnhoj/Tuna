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

namespace Audiochan.Core.Features.Followers.GetFollowers
{
    public record GetFollowersQuery : PaginationQueryRequest<FollowerViewModel>
    {
        public string Username { get; init; }
    }

    public class GetFollowersMappingProfile : Profile
    {
        public GetFollowersMappingProfile()
        {
            CreateMap<FollowedUser, FollowerViewModel>()
                .ForMember(dest => dest.Username, opts =>
                    opts.MapFrom(src => src.Observer.UserName))
                .ForMember(dest => dest.Picture, opts =>
                    opts.MapFrom(src => src.Observer.Picture));
        }
    }

    public class GetFollowersQueryHandler : IRequestHandler<GetFollowersQuery, PagedList<FollowerViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetFollowersQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PagedList<FollowerViewModel>> Handle(GetFollowersQuery request,
            CancellationToken cancellationToken)
        {
            return await _dbContext.FollowedUsers
                .AsNoTracking()
                .Include(u => u.Target)
                .Include(u => u.Observer)
                .Where(u => u.Target.UserName == request.Username.Trim().ToLower())
                .ProjectTo<FollowerViewModel>(_mapper.ConfigurationProvider)
                .PaginateAsync(request, cancellationToken);
        }
    }
}