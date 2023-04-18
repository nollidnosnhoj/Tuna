using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using Tuna.Application.Features.Users.DataLoaders;
using Tuna.Application.Features.Users.Models;

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