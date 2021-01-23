namespace Audiochan.Core.Features.Users.Models
{
    /// <summary>
    /// Returns a bit of data about the user who owns a resource.
    /// </summary>
    public class UserViewModel
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
    }
}