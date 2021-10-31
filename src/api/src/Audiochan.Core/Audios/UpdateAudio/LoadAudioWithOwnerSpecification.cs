﻿using Ardalis.Specification;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Audios
{
    public sealed class LoadAudioWithOwnerSpecification : Specification<Audio>
    {
        public LoadAudioWithOwnerSpecification(long audioId)
        {
            Query.Include(a => a.User);
            Query.Where(a => a.Id == audioId);
        }
    }
}