using Tuna.Application.Features.Users.Models;
using HotChocolate;
using HotChocolate.Types;
using Tuna.Application.Services;

namespace Tuna.GraphQl.Features.Users;

[ExtendObjectType(typeof(UserDto))]
public sealed class UserExtensions
{
    [BindMember(nameof(UserDto.ImageId))]
    public string? GetImageUrl([Parent] UserDto audio, [Service] IImageService imageService)
    {
        if (audio.ImageId is null) return null;
        return imageService.GetImageUrl(audio.ImageId, null);
    }
}