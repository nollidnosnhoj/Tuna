using HotChocolate;

namespace Audiochan.Core.Features.Users.Models;

[GraphQLName("CurrentUser")]
public class CurrentUserDto
{
    public long Id { get; set; }
    public string IdentityId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Picture { get; set; }
}