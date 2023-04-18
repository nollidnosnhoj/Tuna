using Audiochan.Core;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Shared;
using HotChocolate;
using HotChocolate.Types;

namespace Audiochan.API.Features.Users;

[ExtendObjectType(typeof(UserDto))]
public sealed class UserExtensions
{
    [BindMember(nameof(UserDto.Picture))]
    public string? GetImageUrl([Parent] UserDto audio)
    {
        if (audio.Picture is null) return null;
        return MediaLinkConstants.AUDIO_PICTURE + audio.Picture;
    }
}