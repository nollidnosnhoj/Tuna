using System.Collections.Generic;
using Audiochan.Core.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Infrastructure.Identity.Models;

public class AudiochanIdentityUser : IdentityUser
{
    public AudiochanIdentityUser(string userName) : base(userName)
    {
        
    }
    
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    
    public IdentityUserDto ToIdentityUserDto()
    {
        return new IdentityUserDto
        {
            Id = this.Id,
            Email = this.Email,
            UserName = this.UserName
        };
    }
}