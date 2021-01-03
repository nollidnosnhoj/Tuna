using System.Collections.Generic;
using Audiochan.Core.Entities.Base;

namespace Audiochan.Core.Entities
{
    public class Profile : BaseEntity
    {
        public string? About { get; set; }
        public string? Website { get; set; }

        public long UserId { get; set; }
    }
}