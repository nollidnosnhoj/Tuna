using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.Dtos;
using HashidsNet;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using MediatR;

namespace Audiochan.API.Queries;

[ExtendObjectType(OperationType.Query)]
public class AudioQueries
{
    public async Task<AudioDto?> GetAudioAsync(
        string slug, 
        IHashids hashids, 
        AudioQueryService audioService,
        CancellationToken cancellationToken)
    {
        var id = hashids.DecodeSingleLong(slug);
        return await audioService.GetAudioAsync(id, cancellationToken);
    }

    [UseOffsetPaging]
    public async Task<CollectionSegment<AudioDto>> GetUserAudiosAsync(
        long userId,
        IResolverContext resolverContext,
        AudioQueryService audioService,
        CancellationToken cancellationToken)
    {
        var skip = resolverContext.ArgumentOptional<int>("skip");
        var take = resolverContext.ArgumentOptional<int>("take");
        return await audioService.GetUserAudiosAsync(userId, skip.Value, take.Value, cancellationToken);
    }
    
    [UseOffsetPaging]
    public async Task<CollectionSegment<AudioDto>> GetUserFavoriteAudiosAsync(
        long userId,
        IResolverContext resolverContext,
        AudioQueryService audioService,
        CancellationToken cancellationToken)
    {
        var skip = resolverContext.ArgumentOptional<int>("skip");
        var take = resolverContext.ArgumentOptional<int>("take");
        return await audioService.GetUserFavoriteAudiosAsync(userId, skip.Value, take.Value, cancellationToken);
    }
}