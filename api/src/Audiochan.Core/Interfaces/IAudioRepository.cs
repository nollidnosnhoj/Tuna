using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudioList;

namespace Audiochan.Core.Interfaces
{
    public interface IAudioRepository : IGenericRepository<Audio>
    {
        Task<CursorListDto<AudioViewModel>> GetCursorPaginationAsync(ISpecification<Audio, AudioViewModel> specification,
            string? cursor, int size = 30, CancellationToken cancellationToken = default);
    }
}