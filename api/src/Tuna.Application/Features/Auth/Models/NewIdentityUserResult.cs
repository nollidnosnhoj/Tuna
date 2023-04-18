using Microsoft.AspNetCore.Identity;

namespace Tuna.Application.Features.Auth.Models;

public class NewIdentityUserResult : IdentityResult
{
    public string IdentityId { get; set; } = null!;

    public new static NewIdentityUserResult Success(string identityId)
    {
        return new NewIdentityUserResult
        {
            IdentityId = identityId,
            Succeeded = true
        };
    }
}