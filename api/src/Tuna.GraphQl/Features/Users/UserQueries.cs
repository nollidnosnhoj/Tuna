using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using MediatR;
using Tuna.Application.Features.Users.Models;
using Tuna.Application.Features.Users.Queries;

namespace Tuna.GraphQl.Features.Users;

[ExtendObjectType(OperationType.Query)]
public class UserQueries
{
    public async Task<UserDto?> GetUserAsync(long id, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetUserQuery(id), cancellationToken);
    }

    [UseOffsetPaging]
    public async Task<CollectionSegment<UserDto>> GetFollowings(
        long observerId,
        IResolverContext resolverContext,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var skip = resolverContext.ArgumentOptional<int>("skip");
        var take = resolverContext.ArgumentOptional<int>("take");
        return await mediator.Send(new GetFollowingsQuery(observerId, skip, take), cancellationToken);
    }

    [UseOffsetPaging]
    public async Task<CollectionSegment<UserDto>> GetFollowers(
        long targetId,
        IResolverContext resolverContext,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var skip = resolverContext.ArgumentOptional<int>("skip");
        var take = resolverContext.ArgumentOptional<int>("take");
        return await mediator.Send(new GetFollowersQuery(targetId, skip, take), cancellationToken);
    }
}