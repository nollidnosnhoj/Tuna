using System;

namespace Audiochan.Domain.Abstractions
{
    public interface IAudited
    {
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
    }
}