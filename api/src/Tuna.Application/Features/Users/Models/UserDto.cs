using HotChocolate;

namespace Tuna.Application.Features.Users.Models;

[GraphQLName("User")]
public class UserDto
{
    public long Id { get; init; }
    public string UserName { get; init; } = null!;
    public string? ImageId { get; init; }
}