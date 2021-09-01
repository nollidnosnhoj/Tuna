using System;

namespace Audiochan.Domain.Abstractions
{
    public interface ISoftDeletable
    {
        public DateTime? Deleted { get; set; }
    }
}