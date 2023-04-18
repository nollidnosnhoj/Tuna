using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Tuna.Application.Features.Auth.Models;

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
            Id = Id,
            Email = Email,
            UserName = UserName
        };
    }
}