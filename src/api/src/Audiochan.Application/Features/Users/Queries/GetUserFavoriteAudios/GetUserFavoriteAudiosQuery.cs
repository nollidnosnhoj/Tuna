using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Dtos.Wrappers;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Commons.Interfaces;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace Audiochan.Application.Features.Users.Queries.GetUserFavoriteAudios
{
    public record GetUserFavoriteAudiosQuery : IHasOffsetPage, IQueryRequest<OffsetPagedListDto<AudioDto>>
    {
        public string? Username { get; set; }
        public int Offset { get; init; }
        public int Size { get; init; }
    }
    
    public class GetUserFavoriteAudiosQueryHandler : IRequestHandler<GetUserFavoriteAudiosQuery, OffsetPagedListDto<AudioDto>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetUserFavoriteAudiosQueryHandler(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<OffsetPagedListDto<AudioDto>> Handle(GetUserFavoriteAudiosQuery query, CancellationToken cancellationToken)
        {
            var results = await _dbContext.Users
                .Where(u => u.UserName == query.Username)
                .SelectMany(u => u.FavoriteAudios)
                .OrderByDescending(u => u.Favorited)
                .Select(fa => fa.Audio)
                .ProjectTo<AudioDto>(_mapper.ConfigurationProvider)
                .OffsetPaginateAsync(query.Offset, query.Size, cancellationToken);
            return new OffsetPagedListDto<AudioDto>(results, query.Offset, query.Size);
        }
    }
}