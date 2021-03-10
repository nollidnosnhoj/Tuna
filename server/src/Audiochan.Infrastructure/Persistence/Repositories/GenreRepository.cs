using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public GenreRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Genre> GetByInput(string input, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            long genreId = 0;

            if (long.TryParse(input, out var parseResult))
                genreId = parseResult;

            return await _dbContext.Genres
                .Where(genre => genre.Id == genreId || genre.Slug == input.Trim().ToLower())
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}