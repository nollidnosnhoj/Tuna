namespace Audiochan.Core.Features.Auth.Models;

public class IdentityUser
{
    public string Id { get; init; } = default!;
    public string? UserName { get; init; }
    public string? Email { get; init; }
}