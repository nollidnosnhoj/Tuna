using System;

namespace Tuna.Application.Entities.Abstractions
{
    public interface ISoftDeletable
    {
        public DateTime? Deleted { get; set; }
    }
}