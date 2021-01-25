using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Genres.Models;

namespace Audiochan.Core.Interfaces
{
    public interface IGenreService
    {
        Task<List<ListGenreViewModel>> ListGenre(ListGenresQueryParams queryParams, 
            CancellationToken cancellationToken = default);
        Task<Genre> GetGenre(string input, CancellationToken cancellationToken = default);
    }
}