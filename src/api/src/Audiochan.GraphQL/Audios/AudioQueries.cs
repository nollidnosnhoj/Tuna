using Audiochan.Application.Commons.Helpers;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Application.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.GraphQL.Common.Attributes;
using HotChocolate.Resolvers;

namespace Audiochan.GraphQL.Audios;

[ExtendObjectType(OperationTypeNames.Query)]
public class AudioQueries
{
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