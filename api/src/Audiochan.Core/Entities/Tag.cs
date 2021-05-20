using System.Collections.Generic;
using Audiochan.Core.Entities.Base;

namespace Audiochan.Core.Entities
{
    public class Tag
    {
        public Tag()
        {
            Audios = new HashSet<Audio>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Audio> Audios { get; set; }
    }
}