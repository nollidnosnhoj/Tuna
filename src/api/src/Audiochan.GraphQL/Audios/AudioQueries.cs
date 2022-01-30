using Audiochan.Application.Commons.Helpers;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Application.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.GraphQL.Audios.DataLoaders;
using Audiochan.GraphQL.Common.Attributes;
using HotChocolate.Resolvers;

namespace Audiochan.GraphQL.Audios;

[ExtendObjectType(OperationTypeNames.Query)]
public class AudioQueries
{
    public async Task<AudioDto> GetAudioById(
        [ID(nameof(AudioDto))] long id,
        AudioByIdDataLoader audioById,
        CancellationToken cancellationToken = default)
    {
        return await audioById.LoadAsync(id, cancellationToken);
    }
    
    public async Task<IEnumerable<AudioDto>> GetAudioByIds(
        [ID(nameof(AudioDto))] long[] ids,
        AudioByIdDataLoader audioById,
        CancellationToken cancellationToken = default)
    {
        return await audioById.LoadAsync(ids, cancellationToken);
    }

    [UseApplicationDbContext]
    [UseSingleOrDefault]
    public IQueryable<AudioDto> GetAudioBySlug(
        string slug,
        IResolverContext resolverContext,
        [ScopedService] ApplicationDbContext dbContext)
    {
        var audioId = HashIdHelper.DecodeLong(slug);
        return dbContext.Audios
            .Where(a => a.Id == audioId)
            .ProjectTo<Audio, AudioDto>(resolverContext);
    }

    [UseApplicationDbContext]
    [UseSingleOrDefault]
    public IQueryable<AudioDto> GetAudiosByUserId(
        [ID(nameof(UserDto))] long userId,
        IResolverContext resolverContext,
        [ScopedService] ApplicationDbContext dbContext)
    {
        return dbContext.Audios
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Id)
            .ProjectTo<Audio, AudioDto>(resolverContext);
    }
    
    [UseApplicationDbContext]
    public IQueryable<AudioDto> GetAudiosByUsername(
        string userName,
        IResolverContext resolverContext,
        [ScopedService] ApplicationDbContext dbContext)
    {
        return dbContext.Users
            .Where(u => u.UserName == userName)
            .SelectMany(u => u.Audios)
            .OrderByDescending(a => a.Id)
            .ProjectTo<Audio, AudioDto>(resolverContext);
    }

    [UseApplicationDbContext]
    public IQueryable<AudioDto> GetFavoriteAudiosByUserId(
        [ID(nameof(UserDto))] long userId,
        IResolverContext resolverContext,
        [ScopedService] ApplicationDbContext dbContext)
    {
        return dbContext.FavoriteAudios
            .Where(fa => fa.UserId == userId)
            .OrderByDescending(fa => fa.Favorited)
            .Select(fa => fa.Audio)
            .ProjectTo<Audio, AudioDto>(resolverContext);
    }
}