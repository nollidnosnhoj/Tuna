namespace Tuna.Application.Features.Auth.Models;

public class IdentityUserDto
{
    public string Id { get; init; } = default!;
    public string? UserName { get; init; }
    public string? Email { get; init; }
}