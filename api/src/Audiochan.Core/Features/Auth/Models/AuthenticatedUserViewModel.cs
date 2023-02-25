namespace Audiochan.Core.Features.Auth.Models;

public class AuthenticatedUserViewModel
{
    public long Id { get; init; }
    public string IdentityId { get; set; } = string.Empty;
    public string UserName { get; init; } = null!;
    public string? Picture { get; init; }
}