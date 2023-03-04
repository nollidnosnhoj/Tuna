using System;

namespace Audiochan.Core.Entities.Abstractions
{
    public interface ISoftDeletable
    {
        public DateTime? Deleted { get; set; }
    }
}