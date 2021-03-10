using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Genres.ListGenre
{
    public enum ListGenresSort
    {
        Alphabetically,
        Popularity
    }

    public record ListGenreQuery : IRequest<List<GenreViewModel>>
    {
        public ListGenresSort Sort { get; } = default;
    }

    public class ListGenreMappingProfile : Profile
    {
        public ListGenreMappingProfile()
        {
            CreateMap<Genre, GenreViewModel>()
                .ForMember(dest => dest.Count, opts =>
                    opts.MapFrom(src => src.Audios.Count));
        }
    }

    public class ListGenreQueryHandler : IRequestHandler<ListGenreQuery, List<GenreViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ListGenreQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<GenreViewModel>> Handle(ListGenreQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Genre> queryable = _dbContext.Genres
                .Include(g => g.Audios);

            switch (request.Sort)
            {
                case ListGenresSort.Popularity:
                    queryable = queryable.OrderByDescending(g => g.Audios.Count);
                    break;
                case ListGenresSort.Alphabetically:
                    queryable = queryable.OrderBy(g => g.Name);
                    break;
            }

            return await queryable
                .ProjectTo<GenreViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
}