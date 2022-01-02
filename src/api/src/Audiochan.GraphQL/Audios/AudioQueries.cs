using System.Security.Claims;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Commons.Helpers;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.GraphQL.Common.Attributes;
using HotChocolate.AspNetCore.Authorization;
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
    [UsePaging]
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
    [UsePaging]
    public IQueryable<AudioDto> GetFavoriteAudiosByUserName(
        string userName,
        IResolverContext resolverContext,
        [ScopedService] ApplicationDbContext dbContext)
    {
        return dbContext.Users
            .Where(u => u.UserName == userName)
            .SelectMany(u => u.FavoriteAudios)
            .OrderByDescending(fa => fa.Favorited)
            .Select(fa => fa.Audio)
            .ProjectTo<Audio, AudioDto>(resolverContext);
    }

    [UseApplicationDbContext]
    [UsePaging]
    public IQueryable<AudioDto> GetAudiosByTags(
        string[] tags,
        IResolverContext resolverContext,
        [ScopedService] ApplicationDbContext dbContext)
    {
        return dbContext.Audios
            .Where(a => a.Tags.Any(tags.Contains))
            .OrderByDescending(a => a.Id)
            .ProjectTo<Audio, AudioDto>(resolverContext);
    }

    [Authorize]
    [UseApplicationDbContext]
    [UsePaging]
    public IQueryable<AudioDto> GetYourAudios(
        ClaimsPrincipal claimsPrincipal,
        IResolverContext resolverContext,
        [ScopedService] ApplicationDbContext dbContext)
    {
        var userId = claimsPrincipal.GetUserId();
        return dbContext.Audios
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Id)
            .ProjectTo<Audio, AudioDto>(resolverContext);
    }
    
    [Authorize]
    [UseApplicationDbContext]
    [UsePaging]
    public IQueryable<AudioDto> GetYourFavoriteAudios(
        ClaimsPrincipal claimsPrincipal,
        IResolverContext resolverContext,
        [ScopedService] ApplicationDbContext dbContext)
    {
        var userId = claimsPrincipal.GetUserId();
        return dbContext.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.FavoriteAudios)
            .OrderByDescending(fa => fa.Favorited)
            .Select(fa => fa.Audio)
            .ProjectTo<Audio, AudioDto>(resolverContext);
    }
}