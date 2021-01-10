using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Entities
{
    public class Role : IdentityRole<long>
    {
        public Role(string role) : base(role) { }
    }
}