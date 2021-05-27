using System;
using Ardalis.Specification;
using Audiochan.API.Mappings;
using Audiochan.Core.Entities;

namespace Audiochan.API.Features.Audios.GetAudio
{
    public sealed class GetAudioSpecification : Specification<Audio, AudioDetailViewModel>
    {
        public GetAudioSpecification(Guid audioId)
        {
            Query.AsNoTracking()
                .Include(a => a.Tags)
                .Include(a => a.User)
                .Where(a => a.Id == audioId);

            Query.Select<Audio, AudioDetailViewModel>(AudioMappings.AudioToDetailProjection());
        }
    }
}