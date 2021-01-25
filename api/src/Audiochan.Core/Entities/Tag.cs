using System.Collections.Generic;

namespace Audiochan.Core.Entities
{
    public class Tag
    {
        public Tag()
        {
            Audios = new HashSet<Audio>();
        }

        public string Id { get; set; }
        public ICollection<Audio> Audios { get; set; }
    }
}
