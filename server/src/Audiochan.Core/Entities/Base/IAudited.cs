using System;
using NodaTime;

namespace Audiochan.Core.Entities.Base
{
    public interface IAudited
    {
        public Instant Created { get; set; }
        public Instant? LastModified { get; set; }
    }
}