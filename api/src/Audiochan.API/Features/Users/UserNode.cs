using Audiochan.Core;
using Audiochan.Core.Features.Users.Models;
using HotChocolate;
using HotChocolate.Types;

namespace Audiochan.API.Features.Users;

[ExtendObjectType<UserViewModel>]
public class UserNode
{
    [BindMember(nameof(UserViewModel.Picture))]
    public string? GetImageUrl([Parent] UserViewModel user)
    {
        if (user.Picture is null) return null;
        return MediaLinkConstants.USER_PICTURE + user.Picture;
    }
}