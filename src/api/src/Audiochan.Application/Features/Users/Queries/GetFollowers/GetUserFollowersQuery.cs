﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Dtos.Wrappers;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Commons.Interfaces;
using Audiochan.Application.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace Audiochan.Application.Features.Users.Queries.GetFollowers
{
    public record GetUserFollowersQuery(string Username) : IHasOffsetPage, IQueryRequest<OffsetPagedListDto<FollowerViewModel>>
    {
        public int Offset { get; init; }
        public int Size { get; init; }
    }

    public class GetUserFollowersQueryHandler : IRequestHandler<GetUserFollowersQuery, OffsetPagedListDto<FollowerViewModel>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetUserFollowersQueryHandler(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<OffsetPagedListDto<FollowerViewModel>> Handle(GetUserFollowersQuery query,
            CancellationToken cancellationToken)
        {
            var list = await _dbContext.FollowedUsers
                .Where(fu => fu.Target.UserName == query.Username)
                .OrderByDescending(fu => fu.FollowedDate)
                .ProjectTo<FollowerViewModel>(_mapper.ConfigurationProvider)
                .OffsetPaginateAsync(query.Offset, query.Size, cancellationToken);
            return new OffsetPagedListDto<FollowerViewModel>(list, query.Offset, query.Size);
        }
    }
}