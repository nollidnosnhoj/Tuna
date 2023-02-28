using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Auth.Models;

public class NewUserIdentityResult : IdentityResult
{
    public string? IdentityId { get; set; }

    public new static NewUserIdentityResult Success(string identityId)
    {
        return new NewUserIdentityResult
        {
            IdentityId = identityId,
            Succeeded = true
        };
    }
}