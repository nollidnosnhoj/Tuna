using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Interfaces
{
    public interface IGenreRepository
    {
        Task<Genre> GetByInput(string input, CancellationToken cancellationToken = default);
    }
}