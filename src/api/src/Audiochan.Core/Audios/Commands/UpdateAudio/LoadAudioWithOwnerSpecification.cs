using Ardalis.Specification;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Audios.Commands
{
    public sealed class LoadAudioWithOwnerSpecification : Specification<Audio>
    {
        public LoadAudioWithOwnerSpecification(long audioId)
        {
            Query.Include(a => a.Artist);
            Query.Where(a => a.Id == audioId);
        }
    }
}