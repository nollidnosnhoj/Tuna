namespace Audiochan.Core.Features.Users.Models;

public record UserViewModel
{
    public long Id { get; init; }
    public string UserName { get; init; } = null!;
    public string? Picture { get; init; }
}