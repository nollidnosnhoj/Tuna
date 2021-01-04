using System.Collections.Generic;

namespace Audiochan.Core.Features.Users.Models
{
    /// <summary>
    /// Used to return authenticated user information.
    /// </summary>
    public class CurrentUserViewModel
    {
        public long Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public ICollection<string> Roles { get; set; } = new List<string>();
    }
}