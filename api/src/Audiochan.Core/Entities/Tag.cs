using System.Collections.Generic;

namespace Audiochan.Core.Entities
{
    public class Tag
    {
        public Tag()
        {
            Audios = new HashSet<AudioTag>();
        }

        public string Id { get; set; } = null!;
        public ICollection<AudioTag> Audios { get; set; }
    }
}
