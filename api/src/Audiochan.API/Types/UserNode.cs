using Audiochan.Core;
using Audiochan.Core.Features.Users.Dtos;
using HotChocolate;
using HotChocolate.Types;

namespace Audiochan.API.Types;

[ExtendObjectType<UserDto>]
public class UserNode
{
    [BindMember(nameof(UserDto.Picture))]
    public string? GetImageUrl([Parent] UserDto user)
    {
        if (user.Picture is null) return null;
        return MediaLinkConstants.USER_PICTURE + user.Picture;
    }
}