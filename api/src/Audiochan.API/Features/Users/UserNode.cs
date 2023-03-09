using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Users.DataLoaders;
using Audiochan.Core.Features.Users.Models;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace Audiochan.API.Features.Users;

[Node]
[ExtendObjectType(typeof(UserDto))]
public sealed class UserNode
{
    [NodeResolver]
    public static Task<UserDto> GetUserAsync(long id, GetUserDataLoader dataLoader,
        CancellationToken cancellationToken)
    {
        return dataLoader.LoadAsync(id, cancellationToken);
    }
}