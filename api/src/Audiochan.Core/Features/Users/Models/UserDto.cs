using HotChocolate;

namespace Audiochan.Core.Features.Users.Models;

[GraphQLName("User")]
public class UserDto
{
    public long Id { get; init; }
    public string UserName { get; init; } = null!;
    public string? Picture { get; init; }
}