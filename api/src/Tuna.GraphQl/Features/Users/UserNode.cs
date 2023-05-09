using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using Tuna.Application.Features.Users.DataLoaders;
using Tuna.Application.Features.Users.Models;
using Tuna.Application.Services;

namespace Tuna.GraphQl.Features.Users;

[Node]
[ExtendObjectType(typeof(UserDto))]
public static class UserNode
{
    [NodeResolver]
    public static Task<UserDto> GetUserAsync(long id, GetUserDataLoader dataLoader,
        CancellationToken cancellationToken)
    {
        return dataLoader.LoadAsync(id, cancellationToken);
    }
    
    [BindMember(nameof(UserDto.ImageId))]
    public static string? GetImageUrl([Parent] UserDto audio, [Service] IImageService imageService)
    {
        if (audio.ImageId is null) return null;
        return imageService.GetImageUrl(audio.ImageId, null);
    }
}