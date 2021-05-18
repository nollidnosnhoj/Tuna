using System;

namespace Audiochan.Core.Entities.Base
{
    public abstract class BaseEntity<TKey> : IAudited
    {
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
    }
}