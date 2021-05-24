using System;

namespace Audiochan.Core.Entities.Base
{
    public abstract class BaseEntity : IAudited
    {
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
    }
}