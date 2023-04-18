using System.Collections.Generic;
using Tuna.Application.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace Tuna.Infrastructure.Identity.Models;

public class AuthUser : IdentityUser
{
    public AuthUser(string userName) : base(userName)
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