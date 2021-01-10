using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Genres
{
    public class GenreService : IGenreService
    {
        private readonly IAudiochanContext _dbContext;

        public GenreService(IAudiochanContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Genre>> ListGenre(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Genres
                .OrderBy(g => g.Name)
                .ToListAsync(cancellationToken);
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