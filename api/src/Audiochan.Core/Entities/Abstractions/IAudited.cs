using System;

namespace Audiochan.Core.Entities.Abstractions
{
    public interface IAudited
    {
        public string UserId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
    }
}