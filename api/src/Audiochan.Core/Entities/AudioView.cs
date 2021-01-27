using System;

namespace Audiochan.Core.Entities
{
    public class AudioView
    {
        public Guid Id { get; set; }
        public string IpAddress { get; set; }
        public DateTime Created { get; set; }
        public string AudioId { get; set; }
    }
}