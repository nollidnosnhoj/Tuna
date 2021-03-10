using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Entities
{
    public class Role : IdentityRole
    {
        public Role()
        {
        }

        public Role(string name) : base(name)
        {
        }
    }
}