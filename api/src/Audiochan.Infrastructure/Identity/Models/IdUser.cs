using Microsoft.AspNetCore.Identity;

namespace Audiochan.Infrastructure.Identity.Models;

public class IdUser : IdentityUser
{
    public IdUser(string userName) : base(userName)
    {
        
    }
}