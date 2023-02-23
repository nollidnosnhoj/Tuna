using Microsoft.AspNetCore.Identity;

namespace Audiochan.Infrastructure.Security.Models;

public class IdUser : IdentityUser
{
    public IdUser(string userName) : base(userName)
    {
        
    }
}