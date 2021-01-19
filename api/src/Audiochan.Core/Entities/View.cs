using System;
using Audiochan.Core.Entities.Base;

namespace Audiochan.Core.Entities
{
    public class View : BaseEntity
    {
        public Guid Id { get; set; }
        public string IpAddress { get; set; } = null!;
        public string AudioId { get; set; } = null!;
    }
}