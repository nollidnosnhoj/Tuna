using System.Threading;
using System.Threading.Tasks;
using HashidsNet;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using MediatR;
using Tuna.Application.Features.Audios.Models;
using Tuna.Application.Features.Audios.Queries;

namespace Tuna.GraphQl.Features.Audios;

[QueryType]
public static class AudioQueries
{
    public static async Task<AudioDto?> GetAudioAsync(
        string slug,
        IHashids hashids,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var id = hashids.DecodeSingleLong(slug);
        return await mediator.Send(new GetAudioQuery(id), cancellationToken);
    }

    [UseOffsetPaging]
    public static async Task<CollectionSegment<AudioDto>> GetUserAudiosAsync(
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
    public static async Task<CollectionSegment<AudioDto>> GetUserFavoriteAudiosAsync(
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