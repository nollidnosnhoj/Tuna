using System;
using Ardalis.Specification;
using Audiochan.Core.Entities;

namespace Audiochan.API.Features.Audios.UpdateAudio
{
    public sealed class GetAudioForUpdateSpecification : Specification<Audio>
    {
        public GetAudioForUpdateSpecification(Guid audioId)
        {
            Query.Include(a => a.User)
                .Include(a => a.Tags)
                .Where(a => a.Id == audioId);
        }
    }
}