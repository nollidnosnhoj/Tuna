using System;

namespace Audiochan.Core.Entities.Abstractions
{
    public interface IAudited
    {
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
    }
}