using System;

namespace Audiochan.Core.Entities.Base
{
    public abstract class BaseEntity<TKey> : IEntity<TKey>, IAudited
    {
        public TKey Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
    }
}