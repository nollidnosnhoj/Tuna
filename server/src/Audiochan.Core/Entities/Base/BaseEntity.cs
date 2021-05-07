using System;
using NodaTime;

namespace Audiochan.Core.Entities.Base
{
    public abstract class BaseEntity<TKey> : IEntity<TKey>, IAudited
    {
        public TKey Id { get; set; }
        public Instant Created { get; set; }
        public Instant? LastModified { get; set; }
    }
}