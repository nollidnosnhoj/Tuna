using System.Threading;
using System.Threading.Tasks;
using Tuna.Application.Features.Audios;
using Tuna.Application.Features.Audios.Models;
using Tuna.Application.Features.Audios.Queries;
using HashidsNet;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using MediatR;

namespace Tuna.GraphQl.Features.Audios;

[ExtendObjectType(OperationType.Query)]
public class AudioQueries
{
    public async Task<AudioDto?> GetAudioAsync(
        string slug, 
        IHashids hashids, 
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var id = hashids.DecodeSingleLong(slug);
        return await mediator.Send(new GetAudioQuery(id), cancellationToken);
    }

    [UseOffsetPaging]
    public async Task<CollectionSegment<AudioDto>> GetUserAudiosAsync(
        long userId,
        IResolverContext resolverContext,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var skip = resolverContext.ArgumentOptional<int>("skip");
        var take = resolverContext.ArgumentOptional<int>("take");
        return await mediator.Send(new GetUserAudiosQuery(userId, skip, take), cancellationToken);
    }
    
    [UseOffsetPaging]
    public async Task<CollectionSegment<AudioDto>> GetUserFavoriteAudiosAsync(
        long userId,
        IResolverContext resolverContext,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var skip = resolverContext.ArgumentOptional<int>("skip");
        var take = resolverContext.ArgumentOptional<int>("take");
        return await mediator.Send(new GetUserFavoriteAudiosQuery(userId, skip, take), cancellationToken);
    }
}