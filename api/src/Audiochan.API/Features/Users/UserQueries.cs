using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Users;
using Audiochan.Core.Features.Users.Models;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using MediatR;

namespace Audiochan.API.Features.Users;

[ExtendObjectType(OperationType.Mutation)]
public class UserQueries
{
    public async Task<UserViewModel?> GetUserAsync(long id, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetUserQuery(id), cancellationToken);
    }

    [UseOffsetPaging]
    public async Task<CollectionSegment<UserViewModel>> GetFollowings(
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
    public async Task<CollectionSegment<UserViewModel>> GetFollowers(
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