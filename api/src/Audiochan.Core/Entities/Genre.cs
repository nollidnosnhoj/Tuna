using System.Collections.Generic;

namespace Audiochan.Core.Entities
{
    public class Genre
    {
        public Genre()
        {
            Audios = new HashSet<Audio>();
        }
        
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public ICollection<Audio> Audios { get; set; }
    }
}