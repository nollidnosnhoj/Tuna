using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Genres.Models;
using Audiochan.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Genres
{
    public class GenreService : IGenreService
    {
        private readonly IDbContext _dbContext;

        public GenreService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ListGenreViewModel>> ListGenre(ListGenresQueryParams queryParams, 
            CancellationToken cancellationToken = default)
        {
            IQueryable<Genre> queryable = _dbContext.Genres
                .Include(g => g.Audios);

            switch (queryParams.Sort)
            {
                case ListGenresSort.Popularity:
                    queryable = queryable.OrderByDescending(g => g.Audios.Count);
                    break;
                case ListGenresSort.Alphabetically:
                    queryable = queryable.OrderBy(g => g.Name);
                    break;
            }

            return await queryable.Select(genre => new ListGenreViewModel
            {
                Id = genre.Id,
                Name = genre.Name,
                Slug = genre.Slug,
                Count = genre.Audios.Count
            }).ToListAsync(cancellationToken);
        }

        public async Task<Genre?> GetGenre(string? input, CancellationToken cancellationToken = default)
        {
            long genreId = 0;
            
            if (string.IsNullOrWhiteSpace(input))
            {
                input = "misc";
            }
            else
            {
                if (long.TryParse(input, out var parseResult))
                    genreId = parseResult;
            }

            Expression<Func<Genre, bool>> predicate = genre =>
                genre.Id == genreId || genre.Slug == input.Trim().ToLower();

            return await _dbContext.Genres.Where(predicate).FirstOrDefaultAsync(cancellationToken);
        }
    }
}