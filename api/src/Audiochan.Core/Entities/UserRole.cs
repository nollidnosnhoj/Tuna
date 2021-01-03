using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Entities
{
    public class UserRole : IdentityUserRole<long>
    {
        public virtual User User { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }
}