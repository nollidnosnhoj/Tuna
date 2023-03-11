using Microsoft.AspNetCore.Identity;

namespace Audiochan.Infrastructure.Identity.Models;

public class AudiochanIdentityUser : IdentityUser
{
    public AudiochanIdentityUser(string userName) : base(userName)
    {
        
    }
}