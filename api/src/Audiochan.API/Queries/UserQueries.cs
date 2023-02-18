using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Users;
using Audiochan.Core.Features.Users.Dtos;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;

namespace Audiochan.API.Queries;

[ExtendObjectType(OperationType.Mutation)]
public class UserQueries
{
    public async Task<UserDto?> GetUserAsync(long id, UserQueryService userService, CancellationToken cancellationToken)
    {
        return await userService.GetUserAsync(id, cancellationToken);
    }

    [UseOffsetPaging]
    public async Task<CollectionSegment<UserDto>> GetFollowings(
        long observerId,
        IResolverContext resolverContext,
        UserQueryService userService,
        CancellationToken cancellationToken)
    {
        var skip = resolverContext.ArgumentOptional<int>("skip");
        var take = resolverContext.ArgumentOptional<int>("take");
        return await userService.GetFollowings(observerId, skip, take, cancellationToken);
    }
    
    [UseOffsetPaging]
    public async Task<CollectionSegment<UserDto>> GetFollowers(
        long targetId,
        IResolverContext resolverContext,
        UserQueryService userService,
        CancellationToken cancellationToken)
    {
        var skip = resolverContext.ArgumentOptional<int>("skip");
        var take = resolverContext.ArgumentOptional<int>("take");
        return await userService.GetFollowers(targetId, skip, take, cancellationToken);
    }
}