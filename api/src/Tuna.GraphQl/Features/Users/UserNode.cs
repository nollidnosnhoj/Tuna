using System.Threading;
using System.Threading.Tasks;
using Tuna.Application.Features.Users.DataLoaders;
using Tuna.Application.Features.Users.Models;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace Tuna.GraphQl.Features.Users;

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