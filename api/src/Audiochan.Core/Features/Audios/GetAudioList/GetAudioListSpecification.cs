using System.Linq;
using Ardalis.Specification;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Features.Audios.GetAudioList
{
    public sealed class GetAudioListSpecification : Specification<Audio, AudioViewModel>
    {
        public GetAudioListSpecification(int size, string? tag = null)
        {
            Query.Select(AudioMappingExtensions.AudioToListProjection())
                .AsNoTracking()
                .Include(a => a.User)
                .Where(a => a.IsPublic)
                .Take(size);

            if (!string.IsNullOrWhiteSpace(tag))
            {
                Query.Where(a => a.Tags.Any(t => t.Name == tag));
            }
        }
    }
}