using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Interfaces
{
    public interface IGenreService
    {
        Task<List<Genre>> ListGenre(CancellationToken cancellationToken = default);
        Task<Genre?> GetGenre(string? input, CancellationToken cancellationToken = default);
    }
}