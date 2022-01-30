using Audiochan.GraphQL.Audios;
using Audiochan.GraphQL.Audios.DataLoaders;
using Audiochan.GraphQL.Users;
using Audiochan.GraphQL.Users.DataLoaders;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.GraphQL;

public static class RegisterGraphQl
{
    public static IServiceCollection AddGraphQl(this IServiceCollection services)
    {
        services.AddGraphQLServer()
            .AddMutationConventions()
            .AddAuthorization()
            .AddType<AudioType>()
            .AddType<UserType>()
            .AddQueryType()
            .AddTypeExtension<UserQueries>()
            .AddMutationType()
            .AddTypeExtension<AudioMutations>()
            .AddDataLoader<AudioByIdDataLoader>()
            .AddDataLoader<UserByIdDataLoader>()
            .AddFiltering()
            .AddSorting()
            .AddGlobalObjectIdentification();

        return services;
    }
}