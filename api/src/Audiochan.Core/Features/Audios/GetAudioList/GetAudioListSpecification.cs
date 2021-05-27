using System.Linq;
using Ardalis.Specification;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Features.Audios.GetAudioList
{
    public sealed class GetAudioListSpecification : Specification<Audio, AudioViewModel>
    {
        public GetAudioListSpecification(string? tag = null)
        {
            Query.AsNoTracking()
                .Include(a => a.User)
                .Where(a => a.IsPublic);

            if (!string.IsNullOrWhiteSpace(tag))
            {
                Query.Where(a => a.Tags.Any(t => t.Name == tag));
            }

            Query.Select(AudioMappings.AudioToListProjection());
        }
    }
}