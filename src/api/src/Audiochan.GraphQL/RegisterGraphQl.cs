using Audiochan.GraphQL.Audios;
using Audiochan.GraphQL.Audios.DataLoaders;
using Audiochan.GraphQL.Auth;
using Audiochan.GraphQL.Common.Errors;
using Audiochan.GraphQL.Users;
using Audiochan.GraphQL.Users.DataLoaders;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.GraphQL;

public static class RegisterGraphQl
{
    public static IServiceCollection AddGraphQl(this IServiceCollection services)
    {
        services.AddGraphQLServer()
            .InitializeOnStartup()
            .AddMutationConventions(new MutationConventionOptions
            {
                InputTypeNamePattern = "{MutationName}Input",
                PayloadTypeNamePattern = "{MutationName}Payload",
                PayloadErrorTypeNamePattern = "{MutationName}Error"
            })
            .AddAuthorization()
            .AddType<AudioType>()
            .AddType<UserType>()
            .AddQueryType()
            .AddTypeExtension<AudioQueries>()
            .AddTypeExtension<UserQueries>()
            .AddMutationType()
            .AddTypeExtension<AudioMutations>()
            .AddTypeExtension<AuthMutations>()
            .AddTypeExtension<UserMutations>()
            .AddDataLoader<AudioByIdDataLoader>()
            .AddDataLoader<UserByIdDataLoader>()
            .AddDataLoader<FavoritedByAudioIdDataLoader>()
            .AddDataLoader<FavoriteAudiosByUserIdDataLoader>()
            .AddDataLoader<FollowingByUserIdDataLoader>()
            .AddDataLoader<FollowerByUserIdDataLoader>()
            .AddFiltering()
            .AddSorting()
            .AddGlobalObjectIdentification()
            .AddErrorInterfaceType<IGraphQlError>();

        return services;
    }
}