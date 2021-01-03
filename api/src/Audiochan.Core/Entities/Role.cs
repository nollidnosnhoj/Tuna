using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Entities
{
    public class Role : IdentityRole<long>
    {
        public Role()
        {
            Users = new HashSet<UserRole>();
        }
        
        public virtual ICollection<UserRole> Users { get; set; }
    }
}