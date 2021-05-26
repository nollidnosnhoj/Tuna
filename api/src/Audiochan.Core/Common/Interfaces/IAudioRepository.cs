using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudioList;

namespace Audiochan.Core.Common.Interfaces
{
    public interface IAudioRepository : IGenericRepository<Audio>
    {
        Task<CursorList<AudioViewModel>> GetCursorPaginationAsync(ISpecification<Audio, AudioViewModel> specification,
            string? cursor, CancellationToken cancellationToken = default);
    }
}