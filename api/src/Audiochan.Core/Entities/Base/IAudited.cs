using System;

namespace Audiochan.Core.Entities.Base
{
    public interface IAudited
    {
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
    }
}